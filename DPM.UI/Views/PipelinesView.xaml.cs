using System.Windows;
using System.Windows.Controls;
using DPM.Core.Models;

namespace DPM.UI.Views
{
    public partial class PipelinesView : Page
    {
        public PipelinesView()
        {
            InitializeComponent();
            LoadPipelines();
        }

        private async void LoadPipelines()
        {
            var pipelines = await App.PipelineService.GetAllPipelinesAsync();
            PipelinesGrid.ItemsSource = pipelines.ToList();
        }

        private async void CreatePipeline_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PipelineNameBox.Text)) return;

            await App.PipelineService.CreatePipelineAsync(
                PipelineNameBox.Text,
                PipelineDescBox.Text);

            PipelineNameBox.Text = "";
            PipelineDescBox.Text = "";
            LoadPipelines();
        }

        private async void RunPipeline_Click(object sender, RoutedEventArgs e)
        {
            var pipeline = (sender as Button)?.DataContext as Pipeline;
            if (pipeline == null) return;

            await App.PipelineService.RunPipelineAsync(pipeline.Id);
            LoadPipelines();
        }

        private async void DeletePipeline_Click(object sender, RoutedEventArgs e)
        {
            var pipeline = (sender as Button)?.DataContext as Pipeline;
            if (pipeline == null) return;

            await App.PipelineService.DeletePipelineAsync(pipeline.Id);
            LoadPipelines();
        }
        private void DetailPipeline_Click(object sender, RoutedEventArgs e)
        {
            var pipeline = (sender as Button)?.DataContext as Pipeline;
            if (pipeline == null) return;
            NavigationService?.Navigate(new PipelineDetailView(pipeline));
        }
    }
}