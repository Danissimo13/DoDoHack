using System.IO;
using System.Threading.Tasks;

namespace DoDoHack.Services.Abstractions
{
    public interface IFileService
    {
        public Task<string> CreateFileAsync(string filePath, Stream stream);
    }
}
