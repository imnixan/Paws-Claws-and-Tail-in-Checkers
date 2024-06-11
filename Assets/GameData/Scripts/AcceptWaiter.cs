using System;
using System.Threading;
using UnityEngine.DedicatedServer;

namespace PTCP.scripts
{
    public class AcceptWaiter<T> : IDisposable
    {
        private Timer _timer;
        private readonly Action _action;
        private readonly int _interval;
        private readonly T _argument;

        public AcceptWaiter(Action action, T argument, int intervalInSeconds)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _argument = argument;
            _interval = intervalInSeconds * 1000;
        }

        public void Start()
        {
            _timer = new Timer(ExecuteAction, null, 0, _interval);
        }

        private void ExecuteAction(object state)
        {
            _action();
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);
        }

        public void Dispose()
        {
            Stop();
            _timer?.Dispose();
        }
    }
}
