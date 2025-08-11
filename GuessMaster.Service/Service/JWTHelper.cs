using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Service.Interface;

namespace GuessMaster.Service.Service
{
    public class JWTHelper: IJWTHelper
    {
        private readonly IConfiguration _config;

        public JWTHelper(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GenerateJwt(int userId, string username, string avatarId, int sessionId = 0)
        {
            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims;

            if (sessionId <= 0)
            {
                claims = new[]
                {
                new Claim("userId", userId.ToString()),
                new Claim("username", username),
                new Claim("avatarId", avatarId)
            };
            }
            else
            {
                claims = new[]
                {
                new Claim("userId", userId.ToString()),
                new Claim("username", username),
                new Claim("avatarId", avatarId),
                new Claim("sessionId", sessionId.ToString())
            };
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
