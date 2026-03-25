using System.Windows;
using DPM.Application.Services;
using DPM.Infrastructure.Data;
using DPM.Infrastructure.Repositories;
using AppServices = DPM.Application.Services;

namespace DPM.UI
{
    public partial class App : System.Windows.Application
    {
        public static AppServices.PipelineService PipelineService { get; private set; } = null!;
        public static AppServices.TransformService TransformService { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var context = new AppDbContext();
            context.Database.EnsureCreated();

            var repository = new PipelineRepository(context);
            TransformService = new TransformService();
            PipelineService = new AppServices.PipelineService(repository, TransformService);
        }
    }
}