

namespace DPM.Core.Models
{
    public enum DataSourceType
    {
        CSV,
        TXT,
        Excel,
        PostgreSQL,
        MySQL,
        Oracle
    }

    public class DataSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DataSourceType Type { get; set; }

        // Pour fichiers (CSV, TXT, Excel)
        public string? FilePath { get; set; }

        // Pour bases de données
        public string? Host { get; set; }
        public int? Port { get; set; }
        public string? DatabaseName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Query { get; set; }

        public int PipelineId { get; set; }
        public Pipeline? Pipeline { get; set; }
    }
}
