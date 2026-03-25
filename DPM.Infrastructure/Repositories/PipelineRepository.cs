using Microsoft.EntityFrameworkCore;
using DPM.Core.Interfaces;
using DPM.Core.Models;
using DPM.Infrastructure.Data;

namespace DPM.Infrastructure.Repositories
{
    public class PipelineRepository : IPipelineRepository
    {
        private readonly AppDbContext _context;

        public PipelineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pipeline>> GetAllAsync()
        {
            return await _context.Pipelines
                .Include(p => p.Jobs)
                .Include(p => p.Logs)
                .Include(p => p.DataSources)
                .Include(p => p.TransformSteps)
                .ToListAsync();
        }

        public async Task<Pipeline?> GetByIdAsync(int id)
        {
            return await _context.Pipelines
                .Include(p => p.Jobs)
                .Include(p => p.Logs)
                .Include(p => p.DataSources)
                .Include(p => p.TransformSteps)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Pipeline pipeline)
        {
            await _context.Pipelines.AddAsync(pipeline);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pipeline pipeline)
        {
            _context.Pipelines.Update(pipeline);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pipeline = await _context.Pipelines.FindAsync(id);
            if (pipeline != null)
            {
                _context.Pipelines.Remove(pipeline);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddDataSourceAsync(DataSource source)
        {
            await _context.DataSources.AddAsync(source);
            await _context.SaveChangesAsync();
        }

        public async Task AddTransformStepAsync(TransformStep step)
        {
            await _context.TransformSteps.AddAsync(step);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransformStepAsync(int stepId)
        {
            var step = await _context.TransformSteps.FindAsync(stepId);
            if (step != null)
            {
                _context.TransformSteps.Remove(step);
                await _context.SaveChangesAsync();
            }
        }
    }
}