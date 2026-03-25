using System.Data;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DPM.Core.Models;

namespace DPM.UI.Views
{
    public partial class PipelineDetailView : Page
    {
        private Pipeline _pipeline;

        public PipelineDetailView(Pipeline pipeline)
        {
            InitializeComponent();
            _pipeline = pipeline;
            LoadPipelineInfo();
        }

        private void LoadPipelineInfo()
        {
            PipelineNameText.Text = $"🔧 {_pipeline.Name}";
            PipelineDescText.Text = _pipeline.Description;
            StatusText.Text = _pipeline.Status;

            (StatusBadge.Background, StatusText.Foreground) = _pipeline.Status switch
            {
                "Success" => (new SolidColorBrush(Color.FromRgb(166, 227, 161)),
                              new SolidColorBrush(Color.FromRgb(30, 30, 46))),
                "Failed" => (new SolidColorBrush(Color.FromRgb(243, 139, 168)),
                              new SolidColorBrush(Color.FromRgb(30, 30, 46))),
                "Running" => (new SolidColorBrush(Color.FromRgb(137, 180, 250)),
                              new SolidColorBrush(Color.FromRgb(30, 30, 46))),
                _ => (new SolidColorBrush(Color.FromRgb(69, 71, 90)),
                              new SolidColorBrush(Colors.White))
            };

            DataSourcesGrid.ItemsSource = _pipeline.DataSources;
            StepsList.ItemsSource = _pipeline.TransformSteps.ToList();
            LogsGrid.ItemsSource = _pipeline.Logs
                .OrderByDescending(l => l.Timestamp).ToList();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PipelinesView());
        }

        private async void RunPipeline_Click(object sender, RoutedEventArgs e)
        {
            await App.PipelineService.RunPipelineAsync(_pipeline.Id);
            _pipeline = (await App.PipelineService.GetPipelineByIdAsync(_pipeline.Id))!;
            LoadPipelineInfo();
            MessageBox.Show("Pipeline executed! ✅", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void LoadRawPreview_Click(object sender, RoutedEventArgs e)
        {
            if (!_pipeline.DataSources.Any())
            {
                MessageBox.Show("No data source attached.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var dt = await App.PipelineService.PreviewDataSourceAsync(
                _pipeline.DataSources.First());
            RawDataGrid.ItemsSource = dt.DefaultView;
        }

        private void TransformType_Changed(object sender, SelectionChangedEventArgs e)
        {
            FilterPanel.Visibility = Visibility.Collapsed;
            SelectColumnsPanel.Visibility = Visibility.Collapsed;
            OrderByPanel.Visibility = Visibility.Collapsed;
            GroupByPanel.Visibility = Visibility.Collapsed;
            ReplacePanel.Visibility = Visibility.Collapsed;
            ChangeTypePanel.Visibility = Visibility.Collapsed;

            var tag = (TransformTypeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            switch (tag)
            {
                case "Filter": FilterPanel.Visibility = Visibility.Visible; break;
                case "SelectColumns": SelectColumnsPanel.Visibility = Visibility.Visible; break;
                case "OrderBy":
                case "OrderByDescending": OrderByPanel.Visibility = Visibility.Visible; break;
                case "GroupBy": GroupByPanel.Visibility = Visibility.Visible; break;
                case "ReplaceValue": ReplacePanel.Visibility = Visibility.Visible; break;
                case "ChangeType": ChangeTypePanel.Visibility = Visibility.Visible; break;
            }
        }

        private async void AddTransformStep_Click(object sender, RoutedEventArgs e)
        {
            var tag = (TransformTypeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            if (tag == null) return;

            var type = Enum.Parse<TransformType>(tag);
            var parameters = new Dictionary<string, string>();

            switch (tag)
            {
                case "Filter":
                    parameters["column"] = FilterColumn.Text;
                    parameters["op"] = (FilterOp.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "==";
                    parameters["value"] = FilterValue.Text;
                    break;
                case "SelectColumns":
                    parameters["columns"] = SelectColumnsBox.Text;
                    break;
                case "OrderBy":
                case "OrderByDescending":
                    parameters["column"] = OrderByColumn.Text;
                    break;
                case "GroupBy":
                    parameters["groupColumn"] = GroupByColumn.Text;
                    parameters["aggColumn"] = AggColumn.Text;
                    parameters["aggFunc"] = (AggFunc.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "SUM";
                    break;
                case "ReplaceValue":
                    parameters["column"] = ReplaceColumn.Text;
                    parameters["oldVal"] = ReplaceOld.Text;
                    parameters["newVal"] = ReplaceNew.Text;
                    break;
                case "ChangeType":
                    parameters["column"] = ChangeTypeColumn.Text;
                    parameters["targetType"] = (TargetTypeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "string";
                    break;
            }

            var step = new TransformStep
            {
                Type = type,
                Parameters = JsonSerializer.Serialize(parameters)
            };

            await App.PipelineService.AddTransformStepAsync(_pipeline.Id, step);
            _pipeline = (await App.PipelineService.GetPipelineByIdAsync(_pipeline.Id))!;
            StepsList.ItemsSource = _pipeline.TransformSteps.ToList();
        }

        private async void ApplyTransform_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dt = await App.PipelineService.GetTransformedPreviewAsync(_pipeline.Id);
                TransformedGrid.ItemsSource = dt.DefaultView;
                TransformStatsText.Text = $"✅ {dt.Rows.Count} rows — {dt.Columns.Count} columns after transforms";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Transform Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteStep_Click(object sender, RoutedEventArgs e)
        {
            var step = (sender as Button)?.DataContext as TransformStep;
            if (step == null) return;

            await App.PipelineService.DeleteTransformStepAsync(step.Id);
            _pipeline = (await App.PipelineService.GetPipelineByIdAsync(_pipeline.Id))!;
            StepsList.ItemsSource = _pipeline.TransformSteps.ToList();
        }

        private async void ClearLogs_Click(object sender, RoutedEventArgs e)
        {
            _pipeline.Logs.Clear();
            await App.PipelineService.UpdatePipelineAsync(_pipeline);
            LogsGrid.ItemsSource = null;
        }
    }
}