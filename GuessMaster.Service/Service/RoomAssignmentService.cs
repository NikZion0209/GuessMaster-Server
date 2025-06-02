using GuessMaster.Data.Models;
using GuessMaster.Repository;
using GuessMaster.Repository.Interface;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Service
{
    public class RoomAssignmentService : IRoomAssignmentService
    {
        private readonly IRepositoryManager _repositoryManager;

        public RoomAssignmentService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public RoomAssignment SaveRoomAssignment(RoomAssignment roomAssignment)
        {
            try
            {
                return _repositoryManager.RoomAssignmentRepository.SaveRoomAssignment(roomAssignment);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
