using ThreadsPoolTest.DotnetControl.Models;

namespace ThreadsPoolTest.DotnetControl.Services;

public class FileBll
{
    private readonly IFileService _fileService;

    public FileBll(IFileService fileService)
    {
        _fileService = fileService;
    }

    public async Task UploadSingleFile(UploadFileRequest request)
    {
        var uploadFile = await GetFileInfo(request.File);
        var uploadFileRequest = new UploadFileDto();

        uploadFileRequest.UploadFiles.Add(uploadFile);

        await _fileService.UploadFiles(uploadFileRequest);
    }

    private async static Task<byte[]> GetBytesFromFile(IFormFile file)
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