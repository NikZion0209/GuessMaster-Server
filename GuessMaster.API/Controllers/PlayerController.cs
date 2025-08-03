using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuessMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public PlayerController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [Route("SavePlayer")]
        [HttpPost]
        public async Task<Result> AddPlayer(User user)
        {
            Result result = new Result();
            try
            {
                var savedUser = _serviceManager.PlayerService.AddPlayer(user);
                Console.WriteLine($"User {savedUser.Username} with ID {savedUser.UserId} saved successfully.");

                // Serialize minimal user info (e.g., UserId and Username)
                var userInfo = new { savedUser.UserId, savedUser.Username, savedUser.AvatarUrl };
                var userInfoJson = System.Text.Json.JsonSerializer.Serialize(userInfo);

                // Set the cookie (HttpOnly, Secure, SameSite)
                Response.Cookies.Append(
                    "UserInfo",
                    userInfoJson,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, // Set to true in production (requires HTTPS)
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(1)
                    }
                );

                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
                result.Data = savedUser;
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
