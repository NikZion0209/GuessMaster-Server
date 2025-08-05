using GuessMaster.Data.Models;
using GuessMaster.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Interface
{
    public interface IPlayerService
    {
        RegisterUserDto AddOrValidateUser(User user, out RegisterUserDto registerUserDto);
        bool RemovePlayer(User user);
        IEnumerable<User> GetAllPlayers();
    }
}
