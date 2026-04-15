using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class DispatchLineService : IDispatchLineService
    {
        private readonly IDispatchLineRepository _repository;

        public DispatchLineService(IDispatchLineRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<dynamic>, int, string)>
            ProcessAsync(DispatchLineRequest request)
        {
            return await _repository.ExecuteAsync(request);
        }
    }
}
