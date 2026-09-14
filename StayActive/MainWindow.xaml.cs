using System.Windows;
using System.Windows.Controls;
using StayActive.Services;
using Application = System.Windows.Application;

namespace StayActive;

public partial class MainWindow : Window
{
    private ActivityService Service => ((App)Application.Current).ActivityService;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) =>
        {
            Service.StateChanged += UpdateStatusText;
            UpdateStatusText(Service.IsActive);
            StartupCheckBox.IsChecked = StartupService.IsEnabled();
        };
    }

    private void StartupCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        StartupService.SetEnabled(StartupCheckBox.IsChecked == true);
    }

    private void UpdateStatusText(bool isActive)
    {
        StatusText.Text = isActive ? "Activo" : "Inactivo";
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

        Service?.SetInterval(seconds);
    }

    private void ActivityTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
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