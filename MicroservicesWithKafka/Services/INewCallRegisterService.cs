using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Services
{
    public interface INewCallRegisterService
    {
        /// <summary>
        /// Saves a new complaint and returns the generated ReturnNo.
        /// </summary>
        Task<string?> SaveComplaintAsync(NewCallRegisterModel model);
    }
}
