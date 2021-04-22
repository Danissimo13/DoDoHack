using DoDoModels;
using System.Threading.Tasks;

namespace DoDoHack.Services.Abstractions
{
    public interface IOrderDistributionService
    {
        public Task<bool> DefineOrderToCourierAsync(Order order);
        public Task<bool> FindOrderToCourierAsync(Courier courier);
    }
}
