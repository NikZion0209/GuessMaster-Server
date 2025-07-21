using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace GuessMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public SessionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
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
        public async Task<Result> AddUserToSession([FromQuery] int gameType, [FromQuery] int sessionId, int userId)
        {
            Result result = new Result();
            try
            {
                Console.WriteLine($"Attempting to add user with ID {userId} to session with ID {sessionId}.");
                _serviceManager.GameService.AddUserToSession(gameType, sessionId, userId);

                // Read existing UserInfo cookie
                var userInfoJson = Request.Cookies["UserInfo"];
                dynamic userInfo;
                if (!string.IsNullOrEmpty(userInfoJson))
                {
                    userInfo = JsonConvert.DeserializeObject<dynamic>(userInfoJson);
                }
                else
                {
                    userInfo = new System.Dynamic.ExpandoObject();
                }

                // Set or update SessionId
                userInfo.SessionId = sessionId;

                // Write back the updated cookie
                var updatedUserInfoJson = JsonConvert.SerializeObject(userInfo);
                Response.Cookies.Append(
                    "UserInfo",
                    updatedUserInfoJson,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, // Set to true in production
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(1)
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
    }
}
