namespace ApiDocAggregator.Services
{
    public class SwaggerSpecBackgroundService : BackgroundService
    {
        private readonly SwaggerSpecService _swaggerSpecService;

        public SwaggerSpecBackgroundService(SwaggerSpecService swaggerSpecService)
        {
            _swaggerSpecService = swaggerSpecService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _swaggerSpecService.DownloadSwaggerSpecsAsync();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Fetch specs every hour
            }
        }
    }
}
