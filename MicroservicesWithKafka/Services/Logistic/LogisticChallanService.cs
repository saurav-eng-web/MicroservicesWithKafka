using MicroservicesWithKafka.Models.Logistic;
using MicroservicesWithKafka.Repository.Logistic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticChallanService : ILogisticChallanService
    {
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

        public async Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload, string? claimSfId)
        {
            var json = LogisticPayload.Parse(payload);
            var type = LogisticPayload.RequireType(json, "asclist", "defective", "doa", "good", "goodmachine", "mis", "misfg");

            if (type == "asclist")
                return (await GetAscListAsync(LogisticPayload.Str(json, "role")), 200, "Record found Successfully");

            var request = new ChallanSearchRequestViewModel
            {
                AscName = LogisticPayload.Str(json, "ascName"),
                SfId = LogisticPayload.Str(json, "sfId"),
                ChallanType = LogisticPayload.Str(json, "challanType"),
                Month = LogisticPayload.RequireInt(json, "month"),
                Year = LogisticPayload.RequireInt(json, "year")
            };

            if (request.Month < 1 || request.Month > 12)
                throw new ArgumentException("month must be between 1 and 12.");
            if (request.Year < 2000 || request.Year > DateTime.Now.Year + 1)
                throw new ArgumentException("year is invalid.");

            switch (type)
            {
                case "defective":
                    if (string.IsNullOrWhiteSpace(request.SfId))
                        throw new ArgumentException("sfId is required (select a service center).");
                    var spType = ResolvePrintChallanType(request.ChallanType)
                        ?? throw new ArgumentException("challanType must be one of Defective, DOA, GoodSpare, GoodFG, MisFG, MisSpare.");
                    return (await _repository.GetPrintChallanListAsync(request.SfId, spType, request.Month, request.Year), 200, "Record found Successfully");

                case "misfg":
                    if (int.TryParse(claimSfId, out var sf) && sf > 0)
                        request.SfId = claimSfId;
                    if (string.IsNullOrWhiteSpace(request.SfId))
                        throw new ArgumentException("sfId is required (select a service center).");
                    return (await _repository.GetMisChallanFGAsync(request), 200, "Record found Successfully");
            }

            if (string.IsNullOrWhiteSpace(request.AscName))
                throw new ArgumentException("ascName is required.");

            object data = type switch
            {
                "doa" => await _repository.GetDOADefectiveChallansAsync(request),
                "good" => await _repository.GetGoodChallansAsync(request),
                "goodmachine" => await _repository.GetGoodMachineChallansAsync(request),
                _ => await _repository.GetMisChallansAsync(request)
            };
            return (data, 200, "Record found Successfully");
        }

        public static string? ResolvePrintChallanType(string? challanType)
        {
            if (string.IsNullOrWhiteSpace(challanType))
                return "Defective";
            return PrintChallanTypes.TryGetValue(challanType.Trim(), out var spType) ? spType : null;
        }

        private async Task<IEnumerable<AscOptionViewModel>> GetAscListAsync(string? role)
        {
            if (string.Equals(role?.Trim(), "LOG", StringComparison.OrdinalIgnoreCase))
                return await _repository.GetLogisticAscListAsync();
            return await _repository.GetAscMasterListAsync();
        }
    }
}
