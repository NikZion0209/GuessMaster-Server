using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessMaster.Service.Interface
{
    public interface IGameTimer
    {
        public void PauseTimer(int sessionId);
        public void ResumeTimer(int sessionId);
        public void CancelTimer(int  sessionId);
        public int GetTimerLength(int sessionId);
        public void SetTimerLength(int sessionId, int timerLength);
        public Task StartTimer(int sessionId, string timerName, int timerLength, int gameType, List<int>? lookoutCondition = null, string? lookoutEvent = null);
    }
}
