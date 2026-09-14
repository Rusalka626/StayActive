using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayActive.Services
{
    public class AppSettings
    {
        public int IntervalSeconds { get; set; } = 30;
        public ActivityType SelectedActivity { get; set; } = ActivityType.MoveMouse;
        public bool ScheduleEnabled { get; set; } = false;
        public TimeSpan ScheduleStart { get; set; } = new TimeSpan(9, 0, 0);
        public TimeSpan ScheduleEnd { get; set; } = new TimeSpan(18, 0, 0);
    }
}
