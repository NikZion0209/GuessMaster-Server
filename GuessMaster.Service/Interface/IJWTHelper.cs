using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Interface
{
    public interface IJWTHelper
    {
        string GenerateJwt(int userId, string username, string avatarId, int premiumTokens, int sessionId = 0);
    }
}
