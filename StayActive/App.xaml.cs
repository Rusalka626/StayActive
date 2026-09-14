using H.NotifyIcon;
using StayActive.Services;
using System.Windows;
using System.Windows.Controls;
using Application = System.Windows.Application;

namespace StayActive
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon? _trayIcon;
        public ActivityService ActivityService { get; private set; } = null!;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var settings = SettingsService.Load();

            ActivityService = new ActivityService(settings.IntervalSeconds, settings.selectedActivity);
            ActivityService.StateChanged += OnActivityStateChanged;

            _trayIcon = (TaskbarIcon)Resources["TrayIcon"];
            _trayIcon.ForceCreate();

            MainWindow?.Hide();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SettingsService.Save(new AppSettings
            {
                IntervalSeconds = ActivityService.IntervalSeconds,
                selectedActivity = ActivityService.SelectedActivity,
            });

            _trayIcon?.Dispose();
            base.OnExit(e);
        }

        private void OnActivityStateChanged(bool isActive)
        {
            var menuItem = ((ContextMenu)_trayIcon!.ContextMenu!).Items[0] as MenuItem;
            menuItem!.Header = isActive ? "Desactivar" : "Activar";
            _trayIcon.ToolTipText = isActive ? "StayActive - Activo" : "StayActive - Inactivo";
        }

        private void ToggleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ActivityService.Toggle();
        }

        private void TrayIcon_LeftClick(object sender, RoutedEventArgs e)
        {
            ActivityService.Toggle();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            MainWindow?.Show();
            MainWindow?.Activate();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _trayIcon?.Dispose();
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
