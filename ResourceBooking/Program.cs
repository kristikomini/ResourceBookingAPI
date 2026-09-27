using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResourceBooking;
using ResourceBooking.Data;
using ResourceBooking.Helpers;
using ResourceBooking.Interfaces;
using ResourceBooking.Middleware;
using ResourceBooking.Models;
using ResourceBooking.Repositories;
using ResourceBooking.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();

// Register repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();

// Register the JWT token service.
builder.Services.AddSingleton<TokenService>();

// Salted, iterated password hashing (PBKDF2) provided by ASP.NET Core Identity.
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// Register AutoMapper (maps between DTOs and entity models).
builder.Services.AddAutoMapper(configuration => { }, typeof(MappingProfiles).Assembly);

// RFC 7807 ProblemDetails responses + a global handler for unhandled exceptions.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Swagger / OpenAPI with support for [SwaggerOperation] annotations.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

// Configure the DbContext to connect to SQL Server.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure()
    );
});

// Read and validate JWT configuration.
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
{
    throw new InvalidOperationException("JWT configuration settings are missing or invalid.");
}

var jwtAudience = builder.Configuration["Jwt:Audience"] ?? jwtIssuer;

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Seed the database when launched with the "seeddata" argument.
if (args.Length == 1 && string.Equals(args[0], "seeddata", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var seed = scope.ServiceProvider.GetRequiredService<Seed>();
    seed.SeedDataContext();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposes the implicit Program class so integration tests can reference the entry point.
public partial class Program { }
