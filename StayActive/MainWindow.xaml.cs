using StayActive.Services;
using System.Windows;

namespace StayActive
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            InputSimulator.JiggleMouse();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }
    }
}