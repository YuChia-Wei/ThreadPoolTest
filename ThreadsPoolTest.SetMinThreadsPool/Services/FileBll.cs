using ThreadsPoolTest.CrossCutting.Observability.Tracing;
using ThreadsPoolTest.SetMinThreadsPool.Models;
using ThreadsPoolTest.UseCases.Files;
using ThreadsPoolTest.UseCases.Files.Dtos;

namespace ThreadsPoolTest.SetMinThreadsPool.Services;

[TracingMethod]
public class FileBll
{
    private readonly IFileService _fileService;

    public FileBll(IFileService fileService)
    {
        this._fileService = fileService;
    }

    public async Task UploadMultipleFilesStreamAsync(IEnumerable<IFormFile> files)
    {
        var uploadFilesDto = new UploadMultipleFilesDto();
        foreach (var file in files)
        {
            uploadFilesDto.Files.Add(new UploadStreamFile
            {
                FileName = file.FileName,
                FileStream = file.OpenReadStream()
            });
        }

        await this._fileService.UploadMultipleFileStreamsAsync(uploadFilesDto);
    }

    public async Task UploadRawFileStreamAsync(Stream fileStream, string fileName)
    {
        var uploadStreamFile = new UploadStreamFile
        {
            FileStream = fileStream,
            FileName = fileName
        };
        await this._fileService.UploadFileStreamAsync(uploadStreamFile);
    }

    public async Task UploadSingleFileAsync(UploadFileRequest request)
    {
        var uploadFile = await GetFileInfoAsync(request.File);
        var uploadFileRequest = new UploadFileDto();

        uploadFileRequest.UploadFiles.Add(uploadFile);

        await this._fileService.UploadFilesAsync(uploadFileRequest);
    }

    public async Task UploadSingleFileStreamAsync(UploadFileRequest request)
    {
        var uploadStreamFile = new UploadStreamFile
        {
            FileStream = request.File.OpenReadStream(),
            FileName = request.File.FileName
        };
        await this._fileService.UploadFileStreamAsync(uploadStreamFile);
    }

    private static async Task<byte[]> GetBytesFromFileAsync(IFormFile file)
    {
        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        return ms.ToArray();
    }

    private static async Task<UploadFile> GetFileInfoAsync(IFormFile file)
    {
        return new UploadFile
        {
            FileExtensions = Path.GetExtension(file.FileName).TrimStart('.').ToLower(),
            FileName = Path.GetFileNameWithoutExtension(file.FileName).ToLower(),
            FileContent = await GetBytesFromFileAsync(file)
        };
    }
}