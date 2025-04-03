
using ApiDocAggregator.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;

namespace ApiDocAggregator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Aggregator",
                    License = new OpenApiLicense() { Name = "MIT License", Url = new Uri("https://opensource.org/licenses/MIT") }
                });
            });

            //builder.Services.AddHttpClient<SwaggerSpecService>();
            builder.Services.AddHttpClient<SwaggerSpecService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost");
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                return handler;
            });
            builder.Services.AddHostedService<SwaggerSpecBackgroundService>();

            var app = builder.Build();
            app.UseStaticFiles();
            // Configure the HTTP request pipeline.
            if (true)
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "swagger")),
                    RequestPath = "/swagger"
                });
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("https://localhost:7157/swagger/Project1_v1.json", "Project 1 API");
                    c.SwaggerEndpoint("https://localhost:7157/swagger/Project2_v1.json", "Project 2 API");
                    c.RoutePrefix = string.Empty; // Serve UI at root
                });


            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
