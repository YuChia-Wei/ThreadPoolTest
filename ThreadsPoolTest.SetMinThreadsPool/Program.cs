using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ThreadsPoolTest.CrossCutting.Observability.Metrics;
using ThreadsPoolTest.SetMinThreadsPool.Models;
using ThreadsPoolTest.SetMinThreadsPool.Services;
using ThreadsPoolTest.UseCases.Files;
using ThreadsPoolTest.UseCases.Files.Services;

ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
ThreadPool.SetMinThreads(128, 128);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
       .WithLogging(loggerProviderBuilder =>
       {
           loggerProviderBuilder.AddOtlpExporter();
       })
       .WithTracing(tracerProviderBuilder =>
       {
           tracerProviderBuilder
               .AddSource(ApplicationMeter.Meter.Name)
               // .AddHttpClientInstrumentation()
               .AddAspNetCoreInstrumentation()
               // .AddEntityFrameworkCoreInstrumentation()
               .AddOtlpExporter();
       })
       .WithMetrics(meterProviderBuilder =>
       {
           meterProviderBuilder
               .AddMeter(ApplicationMeter.Meter.Name)
               .AddRuntimeInstrumentation()
               // .AddHttpClientInstrumentation()
               .AddAspNetCoreInstrumentation()
               .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
               {
                   var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                   exporterOptions.Endpoint = !string.IsNullOrWhiteSpace(endpoint)
                                                  ? new Uri(endpoint)
                                                  : new Uri("http://localhost:4317");

                   metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
               });
       });

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<FileBll>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/upload/fromform", async ([FromForm] UploadFileRequest request, [FromServices] FileBll fileBll) =>
   {
       await fileBll.UploadSingleFileAsync(request);
   })
   .WithName("upload from form")
   .DisableAntiforgery()
   .WithOpenApi();

app.Run();