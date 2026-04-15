using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface IMstColorService
    {
        Task<TblMstColor> CreateColorAsync(TblMstColor color);
        Task<bool> UpdateColorAsync(TblMstColor color);
        Task<bool> DeleteColorAsync(decimal colorId);
        Task<TblMstColor?> GetColorByIdAsync(decimal colorId);
        Task<List<TblMstColor>> GetAllColorsAsync();
    }
}
