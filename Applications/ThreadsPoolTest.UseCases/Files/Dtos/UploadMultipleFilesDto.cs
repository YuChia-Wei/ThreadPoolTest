using System.Collections.Generic;
using System.IO;

namespace ThreadsPoolTest.UseCases.Files.Dtos;

public class UploadMultipleFilesDto
{
    public List<UploadStreamFile> Files { get; set; } = [];
}

public class UploadStreamFile
{
    public Stream FileStream { get; set; }
    public string FileName { get; set; }
}