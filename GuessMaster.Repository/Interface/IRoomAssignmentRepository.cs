using GuessMaster.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Interface
{
    public interface IRoomAssignmentRepository
    {
        public RoomAssignment SaveRoomAssignment(RoomAssignment roomAssignment);
    }
}
