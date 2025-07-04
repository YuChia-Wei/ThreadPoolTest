using ThreadsPoolTest.UseCases.Files.Dtos;

namespace ThreadsPoolTest.UseCases.Files;

public interface IFileService
{
    Task UploadFilesAsync(UploadFileDto dto);
    Task UploadFileStreamAsync(UploadStreamFile uploadStreamFile);
    Task UploadMultipleFileStreamsAsync(UploadMultipleFilesDto dto);
}