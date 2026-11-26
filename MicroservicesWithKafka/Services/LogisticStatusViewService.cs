using MicroservicesWithKafka.Repository;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public interface ILogisticStatusViewService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload);
    }

    public class LogisticStatusViewService : ILogisticStatusViewService
    {
        private static readonly string[] AllowedTypes =
            { "DefectivePart", "DefectiveDOA", "GoodPart", "GoodMachine", "MismatchPart", "MismatchMachine" };

        private readonly ILogisticStatusViewRepository _repository;

        public LogisticStatusViewService(ILogisticStatusViewRepository repository)
        {
            _repository = repository;
        }

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, AllowedTypes);

            var data = await _repository.GetStatusViewAsync(type);
            return (data, 200, "Record found Successfully");
        }
    }
}
