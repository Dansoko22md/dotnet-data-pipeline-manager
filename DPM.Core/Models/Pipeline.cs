


namespace DPM.Core.Models
{
    public class Pipeline
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Idle";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<PipelineJob> Jobs { get; set; } = new();
        public List<ExecutionLog> Logs { get; set; } = new();
        public List<DataSource> DataSources { get; set; } = new();
        public List<TransformStep> TransformSteps { get; set; } = new();
    }
}