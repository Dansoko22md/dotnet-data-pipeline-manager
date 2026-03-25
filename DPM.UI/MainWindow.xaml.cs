using System.Windows;
using System.Windows.Controls;
using DPM.UI.Views;

namespace DPM.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new DashboardView());
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            switch (button?.Tag?.ToString())
            {
                case "Dashboard":
                    MainFrame.Navigate(new DashboardView());
                    break;
                case "Pipelines":
                    MainFrame.Navigate(new PipelinesView());
                    break;
                case "Logs":
                    MainFrame.Navigate(new LogsView());
                    break;
                case "Import":
                    MainFrame.Navigate(new ImportDataView());
                    break;
            }
        }
    }
}