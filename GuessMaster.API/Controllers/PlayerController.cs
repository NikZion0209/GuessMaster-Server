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
                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
                result.Data = await _serviceManager.PlayerService.AddPlayerOrAssignmentRoomAsync(user);
            }
            catch (Exception ex)
            {
                result.Header.ResultCode = "500";
                result.Header.ResultDescription = "INTERNAL SERVER ERROR";
                result.Data = ex.Message;
            }
            return result;
        }
    }
}
