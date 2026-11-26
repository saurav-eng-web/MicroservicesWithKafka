using MicroservicesWithKafka.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public interface ILogisticChallanRepository
    {
        Task<IEnumerable<DefectiveChallanViewModel>> GetPrintChallanListAsync(string sfId, string spType, int month, int year);
        Task<IEnumerable<DefectiveChallanViewModel>> GetDOADefectiveChallansAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<GoodPartChallanViewModel>> GetGoodChallansAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<GoodChallanViewModel>> GetGoodMachineChallansAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<MisChallanViewModel>> GetMisChallansAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<GoodChallanViewModel>> GetMisChallanFGAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<AscOptionViewModel>> GetLogisticAscListAsync();
        Task<IEnumerable<AscOptionViewModel>> GetAscMasterListAsync();
    }
}
