using GuessMaster.Data.Data;
using GuessMaster.Repository.Interface;
using GuessMaster.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IPlayerRepository> _lazyPlayerRepository;
        private readonly Lazy<IRoomRepository> _lazyRoomRepository;
        private readonly Lazy<IRoomAssignmentRepository> _lazyRoomAssignmentRepository; 

        public RepositoryManager(ApplicationDbContext _context)
        {
            _lazyPlayerRepository = new Lazy<IPlayerRepository>(() => new PlayerRepository(_context));
            _lazyRoomRepository = new Lazy<IRoomRepository>(() => new RoomRepository(_context));
            _lazyRoomAssignmentRepository = new Lazy<IRoomAssignmentRepository>(() => new RoomAssignmentRepository(_context));
        }
        public IPlayerRepository PlayerRepository => _lazyPlayerRepository.Value;

        public IRoomRepository RoomRepository => _lazyRoomRepository.Value;

        public IRoomAssignmentRepository RoomAssignmentRepository => _lazyRoomAssignmentRepository.Value;
    }
}
