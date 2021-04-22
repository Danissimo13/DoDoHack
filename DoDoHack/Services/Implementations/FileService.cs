using DoDoHack.Services.Abstractions;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DoDoHack.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public FileService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<string> CreateFileAsync(string filePath, Stream stream)
        {
            string fileName = Guid.NewGuid().ToString() + ".jpg";
            string fullPath = Path.Combine(_hostEnvironment.WebRootPath, filePath, fileName);

            using(var writer = new FileStream(fullPath, FileMode.Create))
            {
                await stream.CopyToAsync(writer);
            }

            return fileName;
        }
    }
}
