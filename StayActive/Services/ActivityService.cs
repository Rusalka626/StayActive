using StayActive.Services;
using System.Windows.Threading;

public class ActivityService
{
    private readonly DispatcherTimer _timer = new();
    private readonly DispatcherTimer _scheduleTimer = new(); // 👈 nuevo

    public bool IsActive { get; private set; }
    public ActivityType SelectedActivity { get; set; }
    public int IntervalSeconds { get; private set; }

    public bool ScheduleEnabled { get; set; }
    public TimeSpan ScheduleStart { get; set; }
    public TimeSpan ScheduleEnd { get; set; }

    public event Action<bool>? StateChanged;

    public ActivityService(AppSettings settings)
    {
        IntervalSeconds = settings.IntervalSeconds;
        SelectedActivity = settings.SelectedActivity;
        ScheduleEnabled = settings.ScheduleEnabled;
        ScheduleStart = settings.ScheduleStart;
        ScheduleEnd = settings.ScheduleEnd;

        _timer.Interval = TimeSpan.FromSeconds(IntervalSeconds);
        _timer.Tick += (_, _) => InputSimulator.PerformActivity(SelectedActivity);

        _scheduleTimer.Interval = TimeSpan.FromMinutes(1);
        _scheduleTimer.Tick += (_, _) => CheckSchedule();
        _scheduleTimer.Start(); // siempre corre, pero solo actúa si ScheduleEnabled = true

        CheckSchedule(); // evalúa el estado inicial al arrancar
    }

    private void CheckSchedule()
    {
        if (!ScheduleEnabled) return;

        var now = DateTime.Now.TimeOfDay;
        bool shouldBeActive = ScheduleStart <= ScheduleEnd
            ? now >= ScheduleStart && now < ScheduleEnd
            : now >= ScheduleStart || now < ScheduleEnd; // soporta rangos que cruzan medianoche

        if (shouldBeActive && !IsActive) Toggle();
        else if (!shouldBeActive && IsActive) Toggle();
    }

    public void Toggle()
    {
        IsActive = !IsActive;
        if (IsActive) _timer.Start(); else _timer.Stop();
        StateChanged?.Invoke(IsActive);
    }

    public void SetInterval(int seconds)
    {
        IntervalSeconds = seconds;
        _timer.Interval = TimeSpan.FromSeconds(seconds);
    }
}