using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataPipelineManager.Models
{
    public class Pipeline
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Idle";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<PipelineJob> Jobs { get; set; } = new();
    }
}
