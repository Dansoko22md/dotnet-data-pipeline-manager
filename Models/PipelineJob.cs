using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPipelineManager.Models
{
    public class PipelineJob
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public int PipelineId { get; set; }
        public Pipeline? Pipeline { get; set; }
    }
}
