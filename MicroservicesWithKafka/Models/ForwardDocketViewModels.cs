using System;
using System.Collections.Generic;

namespace MicroservicesWithKafka.Models
{
    public class ForwardDocketUploadViewModel
    {
        public string? ChallanNo { get; set; }
        public string? DocketNo { get; set; }
        public string? DocStatus { get; set; }
        public string? TransDate { get; set; }
        public string? UploadType { get; set; }
        public string? ChallanType { get; set; }
    }

    public class ForwardDocketRow
    {
        public string ChallanNo { get; set; } = string.Empty;
        public string DocketNo { get; set; } = string.Empty;
        public string DocStatus { get; set; } = string.Empty;
        public DateTime TransDate { get; set; }
        public string UploadType { get; set; } = string.Empty;
        public string ChallanType { get; set; } = string.Empty;
    }

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
