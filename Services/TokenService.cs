using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ResourceBooking.Models;

namespace ResourceBooking.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            // Validate configuration settings. Retrives data from the json file
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var expiryMinutes = _config.GetValue<int>("Jwt:ExpiryMinutes", 120);

            //Throw exception if the key or issuer is missing
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentException("JWT configuration settings are missing or invalid.");
            }

            // Creates a security key from the secret key and uses it to create signing credentials with the HMAC SHA-256 algorithm.
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Defines the claims to be included in the JWT token, such as the user's ID, email, and name.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
            };

            //Creates a JwtSecurityToken object using the issuer, claims, expiration time, and signing credentials.
            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            // Converts the JwtSecurityToken object into a string representation of the token and returns it.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
