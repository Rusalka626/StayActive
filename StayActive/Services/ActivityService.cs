using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace StayActive.Services
{
    public class ActivityService
    {
        private readonly DispatcherTimer _timer = new();
        public bool IsActive { get; private set; }

        public event Action<bool>? StateChanged;

        public ActivityService(int intervalSeconds = 30)
        {
            _timer.Interval = TimeSpan.FromSeconds(intervalSeconds);
            _timer.Tick += (_, _) => InputSimulator.JiggleMouse();
        }

        public void Toggle()
        {
            IsActive = !IsActive;
            if (IsActive) _timer.Start(); else _timer.Stop();
            StateChanged?.Invoke(IsActive);
        }

        public void SetInterval(int seconds) => _timer.Interval = TimeSpan.FromSeconds(seconds);
    }
}
