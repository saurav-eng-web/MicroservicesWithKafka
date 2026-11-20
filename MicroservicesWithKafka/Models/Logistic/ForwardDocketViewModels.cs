using System;
using System.Collections.Generic;

namespace MicroservicesWithKafka.Models.Logistic
{
    /// <summary>One Excel row sent by the Forward Logistic Docket Uploader page.</summary>
    public class ForwardDocketUploadViewModel
    {
        public string? ChallanNo { get; set; }
        public string? DocketNo { get; set; }
        /// <summary>In-Transit or Delivered.</summary>
        public string? DocStatus { get; set; }
        /// <summary>yyyy-MM-ddTHH:mm:ss (dd-MM-yyyy and yyyy-MM-dd are accepted too). A future date is not allowed.</summary>
        public string? TransDate { get; set; }
        public string? UploadType { get; set; }
        public string? ChallanType { get; set; }
    }

    /// <summary>A validated row, ready for challdocketstusuploder.</summary>
    public class ForwardDocketRow
    {
        public string ChallanNo { get; set; } = string.Empty;
        public string DocketNo { get; set; } = string.Empty;
        public string DocStatus { get; set; } = string.Empty;
        public DateTime TransDate { get; set; }
        public string UploadType { get; set; } = string.Empty;
        public string ChallanType { get; set; } = string.Empty;
    }

    /// <summary>A row that SP_CRUD_LogisticForwardDocket could not apply (challan / docket not found, bad status).</summary>
    public class ForwardDocketNotAppliedViewModel
    {
        public string ChallanNo { get; set; } = string.Empty;
        public string DocketNo { get; set; } = string.Empty;
        public string DocStatus { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class ForwardDocketUploadResult
    {
        public int Uploaded { get; set; }
        public List<ForwardDocketNotAppliedViewModel> NotApplied { get; set; } = new List<ForwardDocketNotAppliedViewModel>();
    }
}
