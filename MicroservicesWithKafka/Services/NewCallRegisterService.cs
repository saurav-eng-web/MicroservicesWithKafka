using System.Threading.Tasks;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;

namespace MicroservicesWithKafka.Services
{
    public class NewCallRegisterService : INewCallRegisterService
    {
        private readonly INewCallRegisterRepository _repository;

        public NewCallRegisterService(INewCallRegisterRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Saves a new complaint by calling the repository.
        /// </summary>
        public async Task<string?> SaveComplaintAsync(NewCallRegisterModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return await _repository.CreateComplaintAsync(model);
        }
    }
}
