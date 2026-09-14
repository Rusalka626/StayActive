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
        public ActivityType selectedActivity { get; set; } = ActivityType.MoveMouse;
    }
}
