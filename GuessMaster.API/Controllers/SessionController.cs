using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GuessMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IConfiguration _configuration;
        public SessionController(IServiceManager serviceManager, IConfiguration configuration)
        {
            _serviceManager = serviceManager;
            _configuration = configuration;
        }

        [Route("getAvailableSessions")]
        [HttpGet]
        public async Task<Result> GetAvailableSessions([FromQuery] int gameType)
        {
            Result result = new Result();
            try
            {
                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
                result.Data = _serviceManager.GameService.GetAvailableGameSessions(gameType);
            }
            catch (Exception ex)
            {
                result.Header.ResultCode = "500";
                result.Header.ResultDescription = "INTERNAL SERVER ERROR";
                result.Data = ex.Message;
            }
            return await Task.FromResult(result);
        }

        [Route("addUserToSession")]
        [HttpPost]
        public async Task<Result> AddUserToSession([FromQuery] int gameType, [FromQuery] int sessionId)
        {
            Result result = new Result();
            try
            {
                var userId = int.Parse(User.FindFirstValue("userId"));
                var username = User.FindFirstValue("username");
                var avatarId = User.FindFirstValue("avatarId");

                Console.WriteLine($"Attempting to add user with ID {username} to session with ID {sessionId}.");
                _serviceManager.GameService.AddUserToSession(gameType, sessionId, userId);

                var jwtKey = _configuration["Jwt:Key"];
                var jwtIssuer = _configuration["Jwt:Issuer"];
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("userId", userId.ToString()),
                    new Claim("username", username),
                    new Claim("avatarId", avatarId),
                    new Claim("sessionId", sessionId.ToString())
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
                result.Data = new { token = jwtToken };
            }
            catch (Exception ex)
            {
                result.Header.ResultCode = "500";
                result.Header.ResultDescription = "INTERNAL SERVER ERROR";
                result.Data = ex.Message;
            }
            return await Task.FromResult(result);
        }
    }
}
