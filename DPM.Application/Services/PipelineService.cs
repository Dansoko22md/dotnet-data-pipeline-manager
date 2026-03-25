using System.Data;
using System.Text.Json;
using DPM.Core.Interfaces;
using DPM.Core.Models;
using DPM.Infrastructure.Readers;

namespace DPM.Application.Services
{
    public class PipelineService : IPipelineService
    {
        private readonly IPipelineRepository _repository;
        private readonly ITransformService _transformService;

        public PipelineService(IPipelineRepository repository, ITransformService transformService)
        {
            _repository = repository;
            _transformService = transformService;
        }

        public async Task<IEnumerable<Pipeline>> GetAllPipelinesAsync()
            => await _repository.GetAllAsync();

        public async Task<Pipeline?> GetPipelineByIdAsync(int id)
            => await _repository.GetByIdAsync(id);

        public async Task CreatePipelineAsync(string name, string description)
        {
            var pipeline = new Pipeline
            {
                Name = name,
                Description = description,
                Status = "Idle",
                CreatedAt = DateTime.Now
            };
            await _repository.AddAsync(pipeline);
        }

        public async Task RunPipelineAsync(int pipelineId)
        {
            var pipeline = await _repository.GetByIdAsync(pipelineId);
            if (pipeline == null) return;

            pipeline.Status = "Running";
            await _repository.UpdateAsync(pipeline);

            try
            {
                foreach (var source in pipeline.DataSources)
                {
                    var reader = GetReader(source);
                    var data = await reader.ReadAllAsync();
                    var transformed = _transformService.Apply(data, pipeline.TransformSteps);

                    pipeline.Logs.Add(new ExecutionLog
                    {
                        Message = $"Imported {data.Rows.Count} rows, {transformed.Rows.Count} after transforms from '{source.Name}'.",
                        Level = "Info",
                        Timestamp = DateTime.Now,
                        PipelineId = pipeline.Id
                    });
                }

                pipeline.Status = "Success";
                pipeline.Logs.Add(new ExecutionLog
                {
                    Message = $"Pipeline '{pipeline.Name}' executed successfully.",
                    Level = "Info",
                    Timestamp = DateTime.Now,
                    PipelineId = pipeline.Id
                });
            }
            catch (Exception ex)
            {
                pipeline.Status = "Failed";
                pipeline.Logs.Add(new ExecutionLog
                {
                    Message = $"Error: {ex.Message}",
                    Level = "Error",
                    Timestamp = DateTime.Now,
                    PipelineId = pipeline.Id
                });
            }

            await _repository.UpdateAsync(pipeline);
        }

        public async Task DeletePipelineAsync(int id)
            => await _repository.DeleteAsync(id);

        public async Task AddDataSourceAsync(int pipelineId, DataSource source)
        {
            source.PipelineId = pipelineId;
            await _repository.AddDataSourceAsync(source);
        }

        public async Task<DataTable> PreviewDataSourceAsync(DataSource source)
        {
            var reader = GetReader(source);
            return await reader.ReadPreviewAsync(50);
        }

        public async Task AddTransformStepAsync(int pipelineId, TransformStep step)
        {
            var pipeline = await _repository.GetByIdAsync(pipelineId);
            step.PipelineId = pipelineId;
            step.Order = pipeline?.TransformSteps.Count ?? 0;
            await _repository.AddTransformStepAsync(step);
        }

        public async Task<DataTable> GetTransformedPreviewAsync(int pipelineId)
        {
            var pipeline = await _repository.GetByIdAsync(pipelineId);
            if (pipeline == null || !pipeline.DataSources.Any())
                return new DataTable();

            var source = pipeline.DataSources.First();
            var reader = GetReader(source);
            var data = await reader.ReadPreviewAsync(50);
            return _transformService.Apply(data, pipeline.TransformSteps);
        }

        public async Task DeleteTransformStepAsync(int stepId)
            => await _repository.DeleteTransformStepAsync(stepId);

        private IDataSourceReader GetReader(DataSource source)
        {
            return source.Type switch
            {
                DataSourceType.CSV => new CsvDataReader(source.FilePath!),
                DataSourceType.TXT => new CsvDataReader(source.FilePath!),
                DataSourceType.Excel => new ExcelReader(source.FilePath!),
                DataSourceType.PostgreSQL or
                DataSourceType.MySQL or
                DataSourceType.Oracle => new DatabaseReader(source),
                _ => throw new NotSupportedException($"Source type {source.Type} not supported.")
            };
        }

        public async Task UpdatePipelineAsync(Pipeline pipeline)
            => await _repository.UpdateAsync(pipeline);
    }
}