namespace ThreadsPoolTest.SetMinThreadsPool.Models;

public class UploadFile
{
    public string FileName { get; set; }

    public string FileExtensions { get; set; }

    public byte[] FileContent { get; set; }

    public long Length => FileContent?.Length ?? 0;
}