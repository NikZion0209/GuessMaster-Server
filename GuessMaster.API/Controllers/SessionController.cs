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
        public async Task<Result> AddUserToSession([FromQuery] int sessionId, int userId)
        {
            Result result = new Result();
            try
            {
                Console.WriteLine($"Attempting to add user with ID {userId} to session with ID {sessionId}.");
                _serviceManager.GameService.AddUserToSession(sessionId, userId);
                
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
