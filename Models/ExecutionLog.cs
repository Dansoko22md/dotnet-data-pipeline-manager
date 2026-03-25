using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPipelineManager.Models
{
    public class ExecutionLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = "Info";
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int PipelineId { get; set; }
        public Pipeline? Pipeline { get; set; }
    }
}
