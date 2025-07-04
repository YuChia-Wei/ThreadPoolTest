using ThreadsPoolTest.UseCases.Files.Dtos;

namespace ThreadsPoolTest.UseCases.Files;

public interface IFileService
{
    Task UploadFiles(UploadFileDto dto);
}