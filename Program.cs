using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ResourceBooking;
using ResourceBooking.Data;
using ResourceBooking.Helpers;
using ResourceBooking.Interfaces;
using ResourceBooking.Repositories;
using ResourceBooking.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers(); //to handle HTTP request and return responses
        builder.Services.AddTransient<Seed>(); //to create a service that lives for the duration of a single HTTP request
        builder.Services.AddSwaggerGen(c => //to generate Swagger documentation and the lamda expression is used to enable annotations
        {
            c.EnableAnnotations();
        });

        // Register repositories
        builder.Services.AddScoped<IBookingRepository, BookingRepository>(); //a new instance is created for each request, but the same instance is used throught the request
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
        builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();

        // Register TokenService
        builder.Services.AddSingleton<TokenService>(); //the same instance is shared throughout the applications lifetime

        // Register AutoMapper
        builder.Services.AddAutoMapper(configuration => { }, typeof(MappingProfiles).Assembly); //setting up mappings between different data transfer objects (DTOs) and models

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer(); // Registers services to generate metadata for minimal APIs, aiding in the creation of API documentation.
        builder.Services.AddSwaggerGen(); //Registers the Swagger generator, enabling the creation of interactive API documentation.

        // Creating the DbContext to connect with the database
        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.EnableRetryOnFailure());
        });

        // Configure the JWT authentication
        var jwtKey = builder.Configuration["Jwt:Key"]; //they retrieve the key and issuer from the configuration file json
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];

        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
        {
            throw new ArgumentException("JWT configuration settings are missing or invalid.");
        }

        builder
            .Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //sets the default authentication scheme to JWT
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //sets the default challenge scheme to JWT
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; //sets the default authentication scheme to JWT
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //ensures that the token was issued by a trusted authorization server
                    ValidateAudience = true, // audience is valid
                    ValidateLifetime = true, // checks if the token is expired
                    ValidateIssuerSigningKey = true, // checks if the signing key is valid
                    ValidIssuer = jwtIssuer, // sets the issuer
                    ValidAudience = jwtIssuer, // sets the audience
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), // sets the key for validating the tokens signature
                };
            });

        builder.Services.AddAuthorization(); //adds authorization services to the specified IServiceCollection

        // Allow the React dev server (Vite) to call the API from the browser.
        const string FrontendCors = "FrontendCors";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                FrontendCors,
                policy =>
                    policy
                        .WithOrigins("http://localhost:5173", "http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            );
        });

        var app = builder.Build(); //creates an instace of the WebApplication class

        // Seed the database if the argument is provided
        if (args.Length == 1 && args[0].ToLower() == "seeddata")
        {
            SeedData(app);
        }

        // Seed the database.
        void SeedData(IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var seed = scope.ServiceProvider.GetRequiredService<Seed>();
                seed.SeedDataContext();
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) //checks if the application is running in the development environment
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection(); //encryts the data sent between the client and the server

        app.UseCors(FrontendCors); // must run before authentication so preflight requests succeed

        app.UseAuthentication(); // authenticates the users based on the authentication scheme
        app.UseAuthorization(); // checks if the users have the necessary permissions to access the resources

        app.MapControllers(); //maps the controllers to the application

        app.Run();
    }
}
