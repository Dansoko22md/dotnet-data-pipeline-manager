using System.Windows.Controls;

namespace DPM.UI.Views
{
    public partial class DashboardView : Page
    {
        public DashboardView()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            var pipelines = (await App.PipelineService.GetAllPipelinesAsync()).ToList();

            TotalPipelinesText.Text = pipelines.Count.ToString();
            RunningText.Text = pipelines.Count(p => p.Status == "Running").ToString();
            FailedText.Text = pipelines.Count(p => p.Status == "Failed").ToString();
            RecentPipelinesGrid.ItemsSource = pipelines;
        }
    }
}