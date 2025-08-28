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
        RegisterUserDto AddUser(RegistrationPostDto user, out RegisterUserDto registerUserDto);
        RegisterUserDto ValidateUser(RegistrationPostDto user, out RegisterUserDto registerUserDto);
        bool RemovePlayer(User user);
        HighScores GetHighScores(int userId);
    }
}
