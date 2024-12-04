using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Interface
{
    public interface IRoomService
    {
        public Room GetRoomById(int id);
        public Task<Room> SaveRoom(Room room);
        public List<Room> GetAllRoomList();
        public bool RemoveRoom(Room room);
        public Task<Room> JoinOrCreateRoomAsync(int UserId);
        Task<RoomDetail> GetRoomDetailsByRoomIdAsync(int roomId);
        public Task GetLobbyDetailsViaWebSocketAsync(int roomId);
    }
}
