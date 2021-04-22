using DoDoModels;
using System.Threading.Tasks;

namespace DoDoHack.Services.Abstractions
{
    public interface ISupportService
    {
        public Task SendSOSEmailAsync(Courier sender, string locationRef);
    }
}
