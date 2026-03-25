using System.Data;
using System.Windows;
using System.Windows.Controls;
using DPM.Core.Models;
using Microsoft.Win32;

namespace DPM.UI.Views
{
    public partial class ImportDataView : Page
    {
        public ImportDataView()
        {
            InitializeComponent();
            LoadPipelines();
        }

        private async void LoadPipelines()
        {
            var pipelines = await App.PipelineService.GetAllPipelinesAsync();
            PipelineCombo.ItemsSource = pipelines.ToList();
        }

        private void SourceTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceTypeCombo.SelectedItem is ComboBoxItem item)
            {
                bool isFile = item.Tag?.ToString() is "CSV" or "TXT" or "Excel";
                FilePanel.Visibility = isFile ? Visibility.Visible : Visibility.Collapsed;
                DatabasePanel.Visibility = isFile ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            var type = (SourceTypeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            dialog.Filter = type switch
            {
                "CSV" => "CSV Files (*.csv)|*.csv",
                "TXT" => "Text Files (*.txt)|*.txt",
                "Excel" => "Excel Files (*.xlsx)|*.xlsx",
                _ => "All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
                FilePathBox.Text = dialog.FileName;
        }

        private async void Preview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var source = BuildDataSource();
                if (source == null) return;

                PreviewLabel.Text = "Loading preview...";
                var dt = await App.PipelineService.PreviewDataSourceAsync(source);
                PreviewGrid.ItemsSource = dt.DefaultView;
                PreviewLabel.Text = $"Preview — {dt.Rows.Count} rows";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Preview Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveDataSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var source = BuildDataSource();
                if (source == null) return;

                if (PipelineCombo.SelectedItem is not Pipeline pipeline)
                {
                    MessageBox.Show("Please select a pipeline.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await App.PipelineService.AddDataSourceAsync(pipeline.Id, source);
                MessageBox.Show("Data source saved successfully! ✅", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DataSource? BuildDataSource()
        {
            if (SourceTypeCombo.SelectedItem is not ComboBoxItem item)
            {
                MessageBox.Show("Please select a source type.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            var typeStr = item.Tag?.ToString();
            var type = typeStr switch
            {
                "CSV" => DataSourceType.CSV,
                "TXT" => DataSourceType.TXT,
                "Excel" => DataSourceType.Excel,
                "PostgreSQL" => DataSourceType.PostgreSQL,
                "MySQL" => DataSourceType.MySQL,
                "Oracle" => DataSourceType.Oracle,
                _ => throw new Exception("Unknown type")
            };

            bool isFile = typeStr is "CSV" or "TXT" or "Excel";

            if (isFile && string.IsNullOrWhiteSpace(FilePathBox.Text))
            {
                MessageBox.Show("Please select a file.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            return new DataSource
            {
                Name = isFile
                    ? System.IO.Path.GetFileName(FilePathBox.Text)
                    : $"{typeStr}:{DatabaseNameBox.Text}",
                Type = type,
                FilePath = isFile ? FilePathBox.Text : null,
                Host = isFile ? null : HostBox.Text,
                Port = isFile ? null : int.TryParse(PortBox.Text, out var p) ? p : null,
                DatabaseName = isFile ? null : DatabaseNameBox.Text,
                Username = isFile ? null : UsernameBox.Text,
                Password = isFile ? null : PasswordBox.Password,
                Query = isFile ? null : QueryBox.Text
            };
        }
    }
}