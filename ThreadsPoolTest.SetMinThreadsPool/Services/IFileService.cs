using ThreadsPoolTest.SetMinThreadsPool.Models;

namespace ThreadsPoolTest.SetMinThreadsPool.Services;

public interface IFileService
{
    Task UploadFiles(UploadFileDto dto);
}