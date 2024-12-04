using GuessMaster.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Interface
{
    public interface IRoomRepository
    {
        Room GetRoomById(int id);
        Task<Room> SaveRoom(Room room);
        List<Room> GetAllRoomList();
        bool RemoveRoom(Room room);
        Task<Room> AvailableRoomAsync();
        Task<Room> GetRoomDetailsByRoomIdAsync(int roomId);
    }
}
