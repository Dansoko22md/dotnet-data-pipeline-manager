
using DPM.Core.Models;

namespace DPM.Core.Interfaces
{
    public interface IPipelineRepository
    {
        Task<IEnumerable<Pipeline>> GetAllAsync();
        Task<Pipeline?> GetByIdAsync(int id);
        Task AddAsync(Pipeline pipeline);
        Task UpdateAsync(Pipeline pipeline);
        Task DeleteAsync(int id);
        Task AddDataSourceAsync(DataSource source);
        Task AddTransformStepAsync(TransformStep step);
        Task DeleteTransformStepAsync(int stepId);
    }
}