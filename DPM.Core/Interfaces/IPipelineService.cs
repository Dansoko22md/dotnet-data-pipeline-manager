using System.Data;
using DPM.Core.Models;

namespace DPM.Core.Interfaces
{
    public interface IPipelineService
    {
        Task<IEnumerable<Pipeline>> GetAllPipelinesAsync();
        Task<Pipeline?> GetPipelineByIdAsync(int id);
        Task CreatePipelineAsync(string name, string description);
        Task RunPipelineAsync(int pipelineId);
        Task DeletePipelineAsync(int id);
        Task AddDataSourceAsync(int pipelineId, DataSource source);
        Task<DataTable> PreviewDataSourceAsync(DataSource source);
        Task AddTransformStepAsync(int pipelineId, TransformStep step);
        Task<DataTable> GetTransformedPreviewAsync(int pipelineId);
        Task DeleteTransformStepAsync(int stepId);
        Task UpdatePipelineAsync(Pipeline pipeline);
    }
}