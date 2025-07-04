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

    public async Task UploadSingleFileAsync(UploadFileRequest request)
    {
        var uploadFile = await this.GetFileInfoAsync(request.File);
        var uploadFileRequest = new UploadFileDto();

        uploadFileRequest.UploadFiles.Add(uploadFile);

        await this._fileService.UploadFilesAsync(uploadFileRequest);
    }

    private static async Task<byte[]> GetBytesFromFileAsync(IFormFile file)
    {
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return ms.ToArray();
    }

    private async Task<UploadFile> GetFileInfoAsync(IFormFile file)
    {
        return new UploadFile
        {
            FileExtensions = Path.GetExtension(file.FileName).TrimStart('.').ToLower(),
            FileName = Path.GetFileNameWithoutExtension(file.FileName).ToLower(),
            FileContent = await GetBytesFromFileAsync(file)
        };
    }
}