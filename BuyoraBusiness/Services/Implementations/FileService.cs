using BuyoraBusiness.Services.Interfaces;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace BuyoraBusiness.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 2 * 1024 * 1024; // 2MB

        public async Task<string> UploadFileAsync(IFormFile file, string location)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file chosen");

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException("File is too large (max 2MB)");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("File type is not allowed");

            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, location);
            Directory.CreateDirectory(folderPath); 

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(folderPath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"{location}/{fileName}";
        }
        public bool DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            var webRoot = Path.GetFullPath(_webHostEnvironment.WebRootPath);

            var relativePath = filePath.TrimStart('/', '\\');
            var fullPath = Path.GetFullPath(Path.Combine(webRoot, relativePath));

            if (!fullPath.StartsWith(webRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!File.Exists(fullPath))
                return false;

            File.Delete(fullPath);
            return true;
        }
    }
}
