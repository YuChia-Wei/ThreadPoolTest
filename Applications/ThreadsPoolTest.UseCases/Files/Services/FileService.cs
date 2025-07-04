using Microsoft.Extensions.Logging;
using ThreadsPoolTest.CrossCutting.Observability.Tracing;
using ThreadsPoolTest.UseCases.Files.Dtos;

namespace ThreadsPoolTest.UseCases.Files.Services;

[TracingMethod]
public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }

    public Task UploadFiles(UploadFileDto dto)
    {
        _logger.LogInformation("upload file");
        return Task.CompletedTask;
    }
}