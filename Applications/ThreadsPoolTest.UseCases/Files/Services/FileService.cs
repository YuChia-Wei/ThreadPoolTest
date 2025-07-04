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
        this._logger.LogInformation("upload file");
        return Task.CompletedTask;
    }

    public async Task UploadFileStreamAsync(UploadStreamFile uploadStreamFile)
    {
        var filePath = Path.Combine(Path.GetTempPath(), uploadStreamFile.FileName);
        await using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        {
            await uploadStreamFile.FileStream.CopyToAsync(outputStream);
        }

        this._logger.LogInformation("File {FileName} uploaded to {FilePath}", uploadStreamFile.FileName, filePath);
    }

    public async Task UploadMultipleFileStreamsAsync(UploadMultipleFilesDto dto)
    {
        foreach (var file in dto.Files)
        {
            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
            await using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await file.FileStream.CopyToAsync(outputStream);
            }

            this._logger.LogInformation("Multiple file {FileFileName} uploaded to {FilePath}", file.FileName, filePath);
        }
    }
}