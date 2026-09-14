using System.Windows;
using System.Windows.Controls;
using StayActive.Services;
using Application = System.Windows.Application;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace StayActive;

public partial class MainWindow : Window
{
    private ActivityService Service => ((App)Application.Current).ActivityService;
    private bool _isInitializing = true;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            Service.StateChanged += UpdateStatusText;
            UpdateStatusText(Service.IsActive);
            StartupCheckBox.IsChecked = StartupService.IsEnabled();

            IntervalSlider.Value = Service.IntervalSeconds;
            foreach(ComboBoxItem item in ActivityTypeCombo.Items)
            {
                if((string)item.Tag == Service.SelectedActivity.ToString())
                {
                    ActivityTypeCombo.SelectedItem = item;
                    break;
                }
            }

            ScheduleCheckBox.IsChecked = Service.ScheduleEnabled;
            StartTimeBox.Text = Service.ScheduleStart.ToString(@"hh\:mm");
            EndTimeBox.Text = Service.ScheduleEnd.ToString(@"hh\:mm");

            _isInitializing = false;
        };
    }

    private void ScheduleCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (_isInitializing) return;
        Service.ScheduleEnabled = ScheduleCheckBox.IsChecked == true;
    }

    private void ScheduleTime_Changed(object sender, RoutedEventArgs e)
    {
        if (_isInitializing) return;

        if (TimeSpan.TryParse(StartTimeBox.Text, out var start))
            Service.ScheduleStart = start;

        if (TimeSpan.TryParse(EndTimeBox.Text, out var end))
            Service.ScheduleEnd = end;
    }

    private void StartupCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        StartupService.SetEnabled(StartupCheckBox.IsChecked == true);
    }

    private void UpdateStatusText(bool isActive)
    {
        StatusText.Text = isActive ? "Activo" : "Inactivo";
        StatusDot.Fill = isActive
        ? new SolidColorBrush(Color.FromRgb(46, 204, 113))
        : new SolidColorBrush(Color.FromRgb(149, 165, 166));
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        Service.Toggle();
    }

    private void IntervalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        int seconds = (int)e.NewValue;

        if (IntervalValueText != null)
            IntervalValueText.Text = $"{seconds} segundos";
        
        if (_isInitializing) return;
        Service?.SetInterval(seconds);
    }

    private void ActivityTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;

        if (ActivityTypeCombo.SelectedItem is ComboBoxItem item &&
            Enum.TryParse<ActivityType>((string)item.Tag, out var type))
        {
            Service.SelectedActivity = type;
        }
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
        base.OnClosing(e);
    }
}