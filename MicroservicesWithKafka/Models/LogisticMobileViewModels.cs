using System.Collections.Generic;

namespace MicroservicesWithKafka.Models
{
    public class MobileReportRequestViewModel
    {
        public string? Type { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }

    public class MobileChallanPrintViewModel
    {
        public IDictionary<string, object>? Header { get; set; }
        public IEnumerable<IDictionary<string, object>> Lines { get; set; } = new List<IDictionary<string, object>>();
    }

    public class MobileComplaintHistoryViewModel
    {
        public IEnumerable<IDictionary<string, object>> Customer { get; set; } = new List<IDictionary<string, object>>();
        public IEnumerable<IDictionary<string, object>> CustomerAddress { get; set; } = new List<IDictionary<string, object>>();
        public IEnumerable<IDictionary<string, object>> Product { get; set; } = new List<IDictionary<string, object>>();
        public IEnumerable<IDictionary<string, object>> Complaint { get; set; } = new List<IDictionary<string, object>>();
        public IEnumerable<IDictionary<string, object>> ComplaintDetail { get; set; } = new List<IDictionary<string, object>>();
        public IEnumerable<IDictionary<string, object>> History { get; set; } = new List<IDictionary<string, object>>();
        public IEnumerable<IDictionary<string, object>> Documents { get; set; } = new List<IDictionary<string, object>>();
    }
}
