using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Logistic
{
    public class LogisticReportFile
    {
        public string ReportName { get; set; } = string.Empty;
        public List<IDictionary<string, object>> Rows { get; set; } = new();
    }

    public interface ILogisticReportService
    {
        Task<(object? Data, int StatusCode, string StatusMsg)> ProcessAsync(object payload);
    }
}
