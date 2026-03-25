using Microsoft.EntityFrameworkCore;
using DPM.Core.Models;

namespace DPM.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<PipelineJob> PipelineJobs { get; set; }
        public DbSet<ExecutionLog> ExecutionLogs { get; set; }
        public DbSet<DataSource> DataSources { get; set; }
        public DbSet<TransformStep> TransformSteps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=pipeline.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pipeline>()
                .HasMany(p => p.Jobs)
                .WithOne(j => j.Pipeline)
                .HasForeignKey(j => j.PipelineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pipeline>()
                .HasMany(p => p.Logs)
                .WithOne(l => l.Pipeline)
                .HasForeignKey(l => l.PipelineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pipeline>()
                .HasMany(p => p.DataSources)
                .WithOne(d => d.Pipeline)
                .HasForeignKey(d => d.PipelineId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pipeline>()
                .HasMany(p => p.TransformSteps)
                .WithOne(t => t.Pipeline)
                .HasForeignKey(t => t.PipelineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}