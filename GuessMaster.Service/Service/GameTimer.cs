using GuessMaster.Model.Models;
using GuessMaster.Service.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuessMaster.Model.Constants;

namespace GuessMaster.Service.Service
{
    public class GameTimer : IGameTimer
    {
        public static event Action<int, string>? DCLookoutConditionAction;
        public static event Action<int, int>? TimerTick;
        private static readonly ConcurrentDictionary<int, Model.Models.Timer> Timers = new();
        private static readonly ConcurrentDictionary<int, ManualResetEventSlim> PauseEvents = new();

        private bool CreateTimer(int sessionId, string timerName, int timerLength)
        {
            var timer = new Model.Models.Timer
            {
                name = timerName,
                timer = timerLength
            };
            PauseEvents[sessionId] = new ManualResetEventSlim(true); // Not paused initially
            return Timers.TryAdd(sessionId, timer);
        }

        private void RemoveTimer(int sessionId)
        {
            Timers.TryRemove(sessionId, out var _);
            if (PauseEvents.TryRemove(sessionId, out var evt))
                evt.Dispose();
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
        }

        public void PauseTimer(int sessionId)
        {
            if (PauseEvents.TryGetValue(sessionId, out var evt))
                evt.Reset(); // Pause
            GetTimer(sessionId).paused = true;
        }

        public void ResumeTimer(int sessionId)
        {
            if (PauseEvents.TryGetValue(sessionId, out var evt))
                evt.Set(); // Resume
            GetTimer(sessionId).paused = false;
        }

        public void CancelTimer(int sessionId)
        {
            GetTimer(sessionId).cancelled = true;
            if (PauseEvents.TryGetValue(sessionId, out var evt))
                evt.Set(); // Unblock if paused
        }

        public async Task StartTimer(
            int sessionId, 
            string timerName, 
            int timerLength, 
            int gameType, 
            int? lookoutCondition = null, 
            string? lookoutEvent = null
        ) {
            bool timerExists = !(CreateTimer(sessionId, timerName, timerLength));

            if (timerExists)
            {
                Console.WriteLine($"Timer for session {sessionId} already exists. Cannot start a new timer, resuming existing timer");
                ResumeTimer(sessionId);
            }

            Model.Models.Timer sessionTimer = GetTimer(sessionId);
            var pauseEvent = PauseEvents[sessionId];

            while (sessionTimer.timer > 0)
            {
                if (sessionTimer.cancelled)
                {
                    Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} is cancelled");
                    RemoveTimer(sessionId);
                    return;
                }

                pauseEvent.Wait(); // Efficiently wait until resumed

                if (sessionTimer.cancelled)
                {
                    Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} is cancelled");
                    RemoveTimer(sessionId);
                    return;
                }

                if (sessionTimer.paused)
                {
                    Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} is paused at {sessionTimer.timer} seconds.");
                    // Wait for resume, handled by pauseEvent.Wait()
                }

                if (sessionTimer.timer == lookoutCondition && gameType == Gamemodes.DoodleChamp)
                {
                    DCLookoutConditionAction?.Invoke(sessionId, lookoutEvent ?? "Lookout condition met!");
                }

                TimerTick?.Invoke(sessionId, sessionTimer.timer);

                Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} is running. Time left: {sessionTimer.timer} seconds.");
                await Task.Delay(1000); // Sleep for 1 second
                sessionTimer.timer--;
            }

            Console.WriteLine($"Timer {sessionTimer.name} for session {sessionId} has ended.");
            RemoveTimer(sessionId);
        }
    }
}
