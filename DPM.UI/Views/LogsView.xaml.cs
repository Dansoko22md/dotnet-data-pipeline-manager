using System.Windows.Controls;

namespace DPM.UI.Views
{
    public partial class LogsView : Page
    {
        public LogsView()
        {
            InitializeComponent();
            LoadLogs();
        }

        private async void LoadLogs()
        {
            var pipelines = await App.PipelineService.GetAllPipelinesAsync();
            var logs = pipelines.SelectMany(p => p.Logs)
                                .OrderByDescending(l => l.Timestamp)
                                .ToList();
            LogsGrid.ItemsSource = logs;
        }
    }
}