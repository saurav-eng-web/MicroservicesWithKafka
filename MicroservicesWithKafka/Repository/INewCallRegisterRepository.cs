using System.Threading.Tasks;
using MicroservicesWithKafka.Models;

namespace MicroservicesWithKafka.Repository
{
    public interface INewCallRegisterRepository
    {
        /// <summary>
        /// Creates a new complaint record in the database using Complaint_Save_NewCRM procedure.
        /// </summary>
        /// <param name="model">The NewCallRegisterModel object containing all complaint data.</param>
        /// <returns>The ReturnNo (generated complaint number) from the stored procedure.</returns>
        Task<string?> CreateComplaintAsync(NewCallRegisterModel model);
    }
}
