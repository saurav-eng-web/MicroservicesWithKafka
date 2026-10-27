using MicroservicesWithKafka.Models.Logistic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public interface ILogisticChallanService
    {
        /// <summary>Maps the Defective page challan type to the PrintChallanList @Type value; null when unknown.</summary>
        string? ResolvePrintChallanType(string? challanType);

        Task<IEnumerable<DefectiveChallanViewModel>> GetDefectiveChallansAsync(string sfId, string spType, int month, int year);
        Task<IEnumerable<DefectiveChallanViewModel>> GetDOADefectiveChallansAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<GoodPartChallanViewModel>> GetGoodChallansAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<GoodChallanViewModel>> GetGoodMachineChallansAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<MisChallanViewModel>> GetMisChallansAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<GoodChallanViewModel>> GetMisChallanFGAsync(ChallanSearchRequestViewModel request);
        Task<IEnumerable<AscOptionViewModel>> GetAscListAsync(string? role);
    }
}
