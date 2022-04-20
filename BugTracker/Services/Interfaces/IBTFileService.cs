namespace BugTracker.Services.Interfaces
{
    public interface IBTFileService
    {
        string ConvertByteArrayToFile(byte[] fileData, string extension);

        Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);

        string FormatFileSize(long bytes);

        string GetFileIcon(string file);
    }
}
