using GuessMaster.Data.Data;
using GuessMaster.Data.Models;
using GuessMaster.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Repository
{
    public class RoomAssignmentRepository : IRoomAssignmentRepository
    {
        private readonly ApplicationDbContext _context;
        public RoomAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public RoomAssignment SaveRoomAssignment(RoomAssignment roomAssignment)
        {
            try
            {
                _context.RoomAssignments.Add(roomAssignment);
                _context.SaveChanges();
                return roomAssignment;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
