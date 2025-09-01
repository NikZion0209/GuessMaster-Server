using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Service;
using GuessMaster.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
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
        private readonly IJWTHelper _jwtHelper;
        public SessionController(IServiceManager serviceManager, IJWTHelper jwtHelper)
        {
            _serviceManager = serviceManager;
            _jwtHelper = jwtHelper;
        }

        [Route("GetAvailableSessions")]
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

        [Route("AddUserToSession")]
        [HttpPost]
        public async Task<Result> AddUserToSession([FromQuery] int gameType, [FromQuery] int sessionId)
        {
            Result result = new Result();
            try
            {
                var userId = int.Parse(User.FindFirstValue("userId"));
                var username = User.FindFirstValue("username");
                var avatarId = User.FindFirstValue("avatarId");
                var premiumTokens = int.Parse(User.FindFirstValue("premiumTokens"));

                Console.WriteLine($"Attempting to add user with ID {username} to session with ID {sessionId}.");
                _serviceManager.GameService.AddUserToSession(gameType, sessionId, userId);

                var jwtToken = _jwtHelper.GenerateJwt(userId, username, avatarId, premiumTokens, sessionId);

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

        [Route("AddUserToNextAvailableSession")]
        [HttpPost]
        public async Task<Result> AddUserToNextAvailableSession([FromQuery] int gameType)
        {
            Result result = new Result();
            try
            {
                var userId = int.Parse(User.FindFirstValue("userId"));
                var username = User.FindFirstValue("username");
                var avatarId = User.FindFirstValue("avatarId");
                var premiumTokens = int.Parse(User.FindFirstValue("premiumTokens"));

                Console.WriteLine($"Attempting to add user with ID {username} to the next available session for game type {gameType}.");
                _serviceManager.GameService.AddUserToNextAvailableSession(gameType, userId, out int sessionId);

                if (sessionId <= 0)
                {
                    result.Header.ResultCode = "404";
                    result.Header.ResultDescription = "NO AVAILABLE SESSIONS";
                    result.Data = null;
                    return await Task.FromResult(result);
                }

                var jwtToken = _jwtHelper.GenerateJwt(userId, username, avatarId, premiumTokens, sessionId);
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

        [Route("RemoveSessionFromToken")]
        [HttpPost]
        public async Task<Result> RemoveSessionFromToken()
        {
            Result result = new Result();
            try
            {
                var userId = int.Parse(User.FindFirstValue("userId"));
                var username = User.FindFirstValue("username");
                var avatarId = User.FindFirstValue("avatarId");
                var premiumTokens = int.Parse(User.FindFirstValue("premiumTokens"));

                var jwtToken = _jwtHelper.GenerateJwt(userId, username, avatarId, premiumTokens);
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

        [Route("StartSinglePlayerSession")]
        [HttpGet]
        public async Task<Result> StartSinglePlayerSession([FromQuery] int gameType)
        {
            Result result = new Result();
            try
            {
                var userId = User.FindFirstValue("userId");
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                SinglePlayerSessionData sessionData = new SinglePlayerSessionData
                {
                    GameType = gameType,
                    UserId = userId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    IssuedAt = DateTime.UtcNow
                };

                var encryptedSession = _serviceManager.GameService.GenerateSinglePlayerSession(sessionData);

                HttpContext.Response.Cookies.Append(
                    "SinglePlayerSession", // Cookie name
                    encryptedSession,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTime.UtcNow.AddHours(1)
                    }
                );

                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
            }
            catch (Exception ex)
            {
                result.Header.ResultCode = "500";
                result.Header.ResultDescription = "INTERNAL SERVER ERROR";
                result.Data = ex.Message;
            }
            return await Task.FromResult(result);
        }

        [Route("EndSinglePlayerSession")]
        [HttpPost]
        public async Task<Result> EndSinglePlayerSession([FromQuery] int gameType, [FromQuery] int score)
        {
            Result result = new Result();
            try
            {
                if (HttpContext.Request.Cookies.ContainsKey("SinglePlayerSession"))
                {
                    var singlePlayerSessionValue = HttpContext.Request.Cookies["SinglePlayerSession"];

                    var userId = User.FindFirstValue("userId");
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    var userAgent = Request.Headers["User-Agent"].ToString();

                    SinglePlayerSessionData currentSessionData = new SinglePlayerSessionData
                    {
                        GameType = gameType,
                        UserId = userId,
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        IssuedAt = DateTime.UtcNow
                    };

                    _serviceManager.GameService.DecryptSinglePlayerSession(gameType, score, currentSessionData, singlePlayerSessionValue);

                    HttpContext.Response.Cookies.Delete("SinglePlayerSession");

                    result.Header.ResultCode = "200";
                    result.Header.ResultDescription = "SUCCESS";
                }
                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
            }
            catch (Exception ex)
            {
                result.Header.ResultCode = "500";
                result.Header.ResultDescription = "INTERNAL SERVER ERROR";
                result.Data = ex.Message;
            }
            return await Task.FromResult(result);
        }

        [Route("EndSinglePlayerSessionEarly")]
        [HttpDelete]
        public async Task<Result> EndSinglePlayerSessionEarly()
        {
            Result result = new Result();
            try
            {
                if (HttpContext.Request.Cookies.ContainsKey("SinglePlayerSession"))
                {
                    HttpContext.Response.Cookies.Delete("SinglePlayerSession");
                }
                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
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
