using GuessMaster.Repository.Interface;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service
{
    public interface IServiceManager
    {
        IPlayerService PlayerService { get; }
        IRoomService RoomService { get; }
    }
}
