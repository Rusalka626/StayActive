using H.NotifyIcon;
using StayActive.Services;
using System.Windows;
using System.Windows.Controls;

namespace StayActive
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon? _trayIcon;
        private ActivityService _activityService = null!;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _activityService = new ActivityService(intervalSeconds: 30);
            _activityService.StateChanged += OnActivityStateChanged;

            _trayIcon = (TaskbarIcon)Resources["TrayIcon"];
            _trayIcon.ForceCreate();

            // Oculta la ventana principal al iniciar; la app vive en el tray
            MainWindow?.Hide();
        }

        private void OnActivityStateChanged(bool isActive)
        {
            var menuItem = ((ContextMenu)_trayIcon!.ContextMenu!).Items[0] as MenuItem;
            menuItem!.Header = isActive ? "Desactivar" : "Activar";
            _trayIcon.ToolTipText = isActive ? "StayActive - Activo" : "StayActive - Inactivo";
        }

        private void ToggleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _activityService.Toggle();
        }

        private void TrayIcon_LeftClick(object sender, RoutedEventArgs e)
        {
            _activityService.Toggle();
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
