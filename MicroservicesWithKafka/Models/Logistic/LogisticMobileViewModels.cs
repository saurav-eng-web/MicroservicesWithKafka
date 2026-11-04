using System.Collections.Generic;

namespace MicroservicesWithKafka.Models.Logistic
{
    /// <summary>Request for the legacy Mob/reportsm.aspx Excel reports (SP reportsm).</summary>
    public class MobileReportRequestViewModel
    {
        public string? Type { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }

    /// <summary>Legacy Mob/PrintInvoiceDef_NSP.aspx: invoicedetail1m header + ChallanLinem lines.</summary>
    public class MobileChallanPrintViewModel
    {
        public IDictionary<string, object>? Header { get; set; }
        public IEnumerable<IDictionary<string, object>> Lines { get; set; } = new List<IDictionary<string, object>>();
    }

    /// <summary>
    /// Legacy Mob/complainthistory.aspx: sp_ReminderHistory6 result sets shown on the page
    /// (0,1 customer, 2 product, 3,4 complaint, 9 history) + sp_GetSupportingDocumentmah documents.
    /// </summary>
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
