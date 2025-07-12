using GuessMaster.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Interface
{
    public interface IDoodleChamp
    {
        public Task StartGame(int sessionId, List<ConnectedUser> Users);
    }
}
