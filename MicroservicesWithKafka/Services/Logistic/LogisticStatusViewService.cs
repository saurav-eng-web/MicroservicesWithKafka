using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Repository.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticStatusViewService
    {
        Task<IEnumerable<StatusViewViewModel>> GetStatusViewAsync(string type);
    }

    public class LogisticStatusViewService : ILogisticStatusViewService
    {
        private readonly ILogisticStatusViewRepository _repository;

        public LogisticStatusViewService(ILogisticStatusViewRepository repository)
        {
            _repository = repository;
        }

        // Translated from StatusView.aspx.vb grdReverseDamageMismatch binding
        public async Task<IEnumerable<StatusViewViewModel>> GetStatusViewAsync(string type)
        {
            return await _repository.GetStatusViewAsync(type);
        }
    }
}
