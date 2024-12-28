using GuessMaster.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Repository.Interface
{
    public interface IPlayerRepository
    {
        User AddPlayer(User user);
        bool RemovePlayer(User user);
        IEnumerable<User> GetAllPlayers();
    }
}
