using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GuessMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IConfiguration _configuration;

        public PlayerController(IServiceManager serviceManager, IConfiguration configuration)
        {
            _serviceManager = serviceManager;
            _configuration = configuration;
        }

        [Route("SavePlayer")]
        [HttpPost]
        public async Task<Result> AddPlayer(User user)
        {
            Result result = new Result();
            try
            {
                _serviceManager.PlayerService.AddOrValidateUser(user, out var savedUser);
                Console.WriteLine($"User {savedUser.Username} with ID {savedUser.UserId} saved successfully.");

                var jwtKey = _configuration["Jwt:Key"];
                var jwtIssuer = _configuration["Jwt:Issuer"];
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("userId", savedUser.UserId.ToString()),
                    new Claim("username", savedUser.Username),
                    new Claim("avatarId", savedUser.AvatarId)
                };

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: null,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials
                );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
                result.Data = new
                {
                    token = jwtToken
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                result.Header.ResultCode = "401";
                result.Header.ResultDescription = "Unauthorized access. Please check your credentials.";
                result.Data = ex.Message;
            }
            catch (Exception ex)
            {
                result.Header.ResultCode = "500";
                result.Header.ResultDescription = "Something went wrong while logging in. Please try again.";
                result.Data = ex.Message;
            }
            return await Task.FromResult(result);
        }
    }
}
