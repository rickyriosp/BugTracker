using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class BTFileService : IBTFileService
    {
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);

                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Converting Byte Array to File. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                MemoryStream memoryStream = new();

                await file.CopyToAsync(memoryStream);
                byte[] fileData = memoryStream.ToArray();

                memoryStream.Close();
                memoryStream.Dispose();

                return fileData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Accepting Invite. ---> {ex.Message}");
                throw;
            }
        }

        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal fileSize = bytes;

            while (Math.Round(fileSize / 1024) > 0)
            {
                fileSize /= 1024;
                counter++;
            }

            return string.Format("{0:n1}{1}", fileSize, suffixes[counter]);
        }

        public string GetFileIcon(string file)
        {
            string fileImage = "default";

            if (!string.IsNullOrWhiteSpace(file))
            {
                fileImage = Path.GetExtension(file).Replace(".", "");
            }

            return $"/img/contenttype/{fileImage}.png";
        }
    }
}
