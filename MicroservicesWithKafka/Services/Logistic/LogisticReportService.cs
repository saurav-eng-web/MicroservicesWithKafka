using MicroservicesWithKafka.Repository.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticReportService : ILogisticReportService
    {
        private readonly ILogisticReportRepository _repository;

        public LogisticReportService(ILogisticReportRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<dynamic>> GetAvailableDocketReportAsync() => _repository.GetAvailableDocketReportAsync();

        public Task<IEnumerable<dynamic>> GetUsedDocketReportAsync() => _repository.GetUsedDocketReportAsync();

        public Task<IEnumerable<dynamic>> GetCompleteDocketReportAsync() => _repository.GetCompleteDocketReportAsync();

        public Task<IEnumerable<dynamic>> GetTotalDispatchReportAsync(string fromDate, string toDate)
            => _repository.GetTotalDispatchReportAsync(fromDate, toDate);

        public Task<IEnumerable<dynamic>> GetTotalDispatchHeaderReportAsync(string fromDate, string toDate)
            => _repository.GetTotalDispatchHeaderReportAsync(fromDate, toDate);
    }
}
