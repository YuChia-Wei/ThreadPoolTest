using ThreadsPoolTest.DotnetControl.Models;

namespace ThreadsPoolTest.DotnetControl.Services;

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