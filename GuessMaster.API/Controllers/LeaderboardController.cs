using GuessMaster.Model.ViewModel;
using GuessMaster.Service;
using GuessMaster.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuessMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaderboardController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public LeaderboardController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Route("GetTopTenPlayers")]
        [HttpGet]
        public async Task<Result> GetTopTenPlayers([FromQuery] int gameType)
        {
            Result result = new Result();
            try
            {
                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
                result.Data = _serviceManager.LeaderboardService.GetTopTenPlayers(gameType);
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
