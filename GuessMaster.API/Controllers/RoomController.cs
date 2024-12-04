using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using GuessMaster.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Azure.Core.HttpHeader;

namespace GuessMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IServiceManager _servicesManager;
        private readonly HttpContext _httpContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RoomController(IServiceManager servicesManager, IHttpContextAccessor httpContextAccessor)
        {
            _servicesManager = servicesManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("GetRoomDetails")]
        [HttpGet]
        public async Task GetRoomDetails(int roomId)
        {
            Result result = new Result();
            try
            {
                result.Header.ResultCode = "200";
                result.Header.ResultDescription = "SUCCESS";
                var httpContext = _httpContextAccessor.HttpContext;
                //result.Data = await _servicesManager.RoomService.GetLobbyDetailsViaWebSocketAsync(roomId, _httpContext);
                await _servicesManager.RoomService.GetLobbyDetailsViaWebSocketAsync(roomId);
            }
            catch (Exception ex)
            {
                result.Header.ResultCode = "500";
                result.Header.ResultDescription = "INTERNAL SERVER ERROR";
                result.Data = ex.Message;
            }
            //return result;
        }
    }
}
