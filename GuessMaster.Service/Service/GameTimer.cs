using GuessMaster.Model.Models;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuessMaster.Model.Constants;

namespace GuessMaster.Service.Service
{
    public class GameTimer : IGameTimer
    {
        public static event Action<int, string>? DCLookoutConditionAction;
        public static event Action<int, int>? TimerTick;
        private static readonly ConcurrentDictionary<int, Model.Models.Timer> Timers = new();

        private bool CreateTimer(int sessionId, string timerName, int timerLength)
        {
            var timer = new Model.Models.Timer
            {
                name = timerName,
                timer = timerLength
            };
            return Timers.TryAdd(sessionId, timer);
        }

        private void RemoveTimer(int sessionId)
        {
            Timers.TryRemove(sessionId, out var _);
        }

        private Model.Models.Timer GetTimer(int sessionId)
        {
            if (Timers.TryGetValue(sessionId, out var timer))
            {
                return timer;
            }
            throw new KeyNotFoundException($"Timer for session {sessionId} not found.");
        }

        public int GetTimerLength(int sessionId)
        {
            Model.Models.Timer timer = GetTimer(sessionId);
            return timer.timer;
        }

        public void SetTimerLength(int sessionId, int timerLength)
        {
            Model.Models.Timer timer = GetTimer(sessionId);
            timer.timer = timerLength;
            return;
        }

        public void PauseTimer(int sessionId)
        {
            Model.Models.Timer timer = GetTimer(sessionId);
            timer.paused = true;

            return;
        }

        public void UnpauseTimer(int sessionId)
        {
            Model.Models.Timer timer = GetTimer(sessionId);
            timer.paused = false;
            return;
        }

        public void CancelTimer(int sessionId)
        {
            Model.Models.Timer timer = GetTimer(sessionId);
            timer.cancelled = false;

            return;
        }

        public Task StartTimer(int sessionId, string timerName, int timerLength, int gameType, int? lookoutCondition = null, string? lookoutEvent = null)
        {
            bool timerExists = !(CreateTimer(sessionId, timerName, timerLength));

            if (timerExists)
            {
                Console.WriteLine($"Timer for session {sessionId} already exists. Cannot start a new timer, resuming existing timer");
            }

            Model.Models.Timer sessionTimer = GetTimer(sessionId);

            for (int time = sessionTimer.timer; time > 0; time--)
            {
                if (sessionTimer.cancelled)
                {
                    Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} is cancelled");
                    RemoveTimer(sessionId);
                    return Task.CompletedTask;
                }

                if (sessionTimer.paused)
                {
                    Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} is paused at {time} seconds.");
                    sessionTimer.timer = time;
                    return Task.CompletedTask; // Exit if paused
                }

                if (time == lookoutCondition && gameType == Gamemodes.DoodleChamp)
                {
                    DCLookoutConditionAction?.Invoke(sessionId, lookoutEvent ?? "Lookout condition met!");
                }

                TimerTick?.Invoke(sessionId, time);

                Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} is running. Time left: {time} seconds.");
                Thread.Sleep(1000); // Sleep for 1 second
            }

            Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} has ended.");
            RemoveTimer(sessionId);
            return Task.CompletedTask;
        }
    }
}
