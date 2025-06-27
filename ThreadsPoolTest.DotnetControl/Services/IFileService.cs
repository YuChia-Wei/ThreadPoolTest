using ThreadsPoolTest.DotnetControl.Models;

namespace ThreadsPoolTest.DotnetControl.Services;

public interface IFileService
{
    Task UploadFiles(UploadFileDto dto);
}