
namespace DPM.Core.Models
{
    public class ExecutionLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = "Info"; // Info / Warning / Error
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int PipelineId { get; set; }
        public Pipeline? Pipeline { get; set; }
    }
}