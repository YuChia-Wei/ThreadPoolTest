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
        this._logger = logger;
    }

    public Task UploadFilesAsync(UploadFileDto dto)
    {
        this._logger.LogTrace("upload file");
        return Task.CompletedTask;
    }

    public async Task UploadFileStreamAsync(UploadStreamFile uploadStreamFile)
    {
        await using var ms = new MemoryStream();
        await uploadStreamFile.FileStream.CopyToAsync(ms);
        this._logger.LogTrace("File {FileName} uploaded", uploadStreamFile.FileName);
    }

    public async Task UploadMultipleFileStreamsAsync(UploadMultipleFilesDto dto)
    {
        foreach (var file in dto.Files)
        {
            await using var ms = new MemoryStream();
            await file.FileStream.CopyToAsync(ms);
            this._logger.LogTrace("Multiple file {FileFileName} uploaded", file.FileName);
        }
    }
}