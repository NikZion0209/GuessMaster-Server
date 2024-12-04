using GuessMaster.Data.Data;
using GuessMaster.Data.Models;
using GuessMaster.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;
        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Room> AvailableRoomAsync()
        {
            try
            {
                return await _context.Rooms
                    .Where(r => r.IsFull == false || r.IsFull == null)  // Handle nullable bool
                    .OrderBy(r => Guid.NewGuid())   // Randomize selection
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Room> GetAllRoomList()
        {
            try
            {
                return _context.Rooms.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Room GetRoomById(int id)
        {
            try
            {
                return _context.Rooms.Where(x => x.RoomId == id).FirstOrDefault();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Room> GetRoomDetailsByRoomIdAsync(int roomId)
        {
            try
            {
                return await _context.Rooms
                .Include(r => r.RoomAssignments)
                .ThenInclude(ra => ra.User)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool RemoveRoom(Room room)
        {
            try
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Room> SaveRoom(Room room)
        {
            try
            {
                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();
                return room;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
