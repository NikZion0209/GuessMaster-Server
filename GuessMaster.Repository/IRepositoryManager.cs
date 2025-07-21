using GuessMaster.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository
{
    public interface IRepositoryManager
    {
        IPlayerRepository PlayerRepository { get; }
    }
}
