using ThreadsPoolTest.UseCases.Files.Dtos;

namespace ThreadsPoolTest.UseCases.Files;

public interface IFileService
{
    Task UploadFilesAsync(UploadFileDto dto);
}