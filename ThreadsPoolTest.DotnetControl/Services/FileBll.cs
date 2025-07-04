using ThreadsPoolTest.CrossCutting.Observability.Tracing;
using ThreadsPoolTest.DotnetControl.Models;
using ThreadsPoolTest.UseCases.Files;
using ThreadsPoolTest.UseCases.Files.Dtos;

namespace ThreadsPoolTest.DotnetControl.Services;

[TracingMethod]
public class FileBll
{
    private readonly IFileService _fileService;

    public FileBll(IFileService fileService)
    {
        this._fileService = fileService;
    }

    public async Task UploadSingleFile(UploadFileRequest request)
    {
        var uploadFile = await this.GetFileInfo(request.File);
        var uploadFileRequest = new UploadFileDto();

        uploadFileRequest.UploadFiles.Add(uploadFile);

        await this._fileService.UploadFiles(uploadFileRequest);
    }

    private static async Task<byte[]> GetBytesFromFile(IFormFile file)
    {
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return ms.ToArray();
    }

    private async Task<UploadFile> GetFileInfo(IFormFile file)
    {
        var f = new UploadFile
        {
            FileExtensions = Path.GetExtension(file.FileName).TrimStart('.').ToLower(),
            FileName = Path.GetFileNameWithoutExtension(file.FileName).ToLower(),
            FileContent = await GetBytesFromFile(file)
        };

        return f;
    }
}