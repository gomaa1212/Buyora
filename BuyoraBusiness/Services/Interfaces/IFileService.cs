using Microsoft.AspNetCore.Http;

namespace BuyoraBusiness.Services.Interfaces
{
    public interface IFileService
    {
        public Task<string> UploadFileAsync(IFormFile file, string location);
        public bool DeleteFile(string filePath);
    }
}
