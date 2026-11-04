using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository.Logistic
{
    public interface ILogisticChallanRepository
    {
        // Logistic/PrintDefectiveChallan.aspx.vb -> PrintChallanList SP
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
