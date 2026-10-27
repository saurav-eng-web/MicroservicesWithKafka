using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Repository.Logistic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticChallanService : ILogisticChallanService
    {
        // Legacy PrintDefectiveChallan.aspx "Challan Type" dropdown text -> PrintChallanList @Type:
        // Defective -> Defective, DOA -> DOA, Good Spare -> goodspare, GoodFG -> GoodFG,
        // Mis. Challan -> MisFG, Mis. Challan Spare -> MisSpare.
        private static readonly Dictionary<string, string> PrintChallanTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Defective"]          = "Defective",
            ["DOA"]                = "DOA",
            ["GoodSpare"]          = "goodspare",
            ["Good Spare"]         = "goodspare",
            ["GoodFG"]             = "GoodFG",
            ["MisFG"]              = "MisFG",
            ["Mis. Challan"]       = "MisFG",
            ["MisSpare"]           = "MisSpare",
            ["Mis. Challan Spare"] = "MisSpare"
        };

        private readonly ILogisticChallanRepository _repository;

        public LogisticChallanService(ILogisticChallanRepository repository)
        {
            _repository = repository;
        }

        public string? ResolvePrintChallanType(string? challanType)
        {
            if (string.IsNullOrWhiteSpace(challanType))
                return "Defective";
            return PrintChallanTypes.TryGetValue(challanType.Trim(), out var spType) ? spType : null;
        }

        public async Task<IEnumerable<DefectiveChallanViewModel>> GetDefectiveChallansAsync(string sfId, string spType, int month, int year)
        {
            return await _repository.GetPrintChallanListAsync(sfId, spType, month, year);
        }

        public async Task<IEnumerable<DefectiveChallanViewModel>> GetDOADefectiveChallansAsync(ChallanSearchRequestViewModel request)
        {
            return await _repository.GetDOADefectiveChallansAsync(request);
        }

        public async Task<IEnumerable<GoodPartChallanViewModel>> GetGoodChallansAsync(ChallanSearchRequestViewModel request)
        {
            return await _repository.GetGoodChallansAsync(request);
        }

        public async Task<IEnumerable<GoodChallanViewModel>> GetGoodMachineChallansAsync(ChallanSearchRequestViewModel request)
        {
            return await _repository.GetGoodMachineChallansAsync(request);
        }

        public async Task<IEnumerable<MisChallanViewModel>> GetMisChallansAsync(ChallanSearchRequestViewModel request)
        {
            return await _repository.GetMisChallansAsync(request);
        }

        public async Task<IEnumerable<GoodChallanViewModel>> GetMisChallanFGAsync(ChallanSearchRequestViewModel request)
        {
            return await _repository.GetMisChallanFGAsync(request);
        }

        /// <summary>role=LOG -> ASClist(@Role='LOG') (Defective page); otherwise full ASC_MASTER (DOA/Good/Mis pages).</summary>
        public async Task<IEnumerable<AscOptionViewModel>> GetAscListAsync(string? role)
        {
            if (string.Equals(role?.Trim(), "LOG", StringComparison.OrdinalIgnoreCase))
                return await _repository.GetLogisticAscListAsync();
            return await _repository.GetAscMasterListAsync();
        }
    }
}
