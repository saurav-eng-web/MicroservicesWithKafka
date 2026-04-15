using System;

namespace MicroservicesWithKafka.Models
{
    public class ActionMaster
    {
        public int? ID { get; set; }
        public string? Type { get; set; }
        public string? Action { get; set; }
        public string? Role { get; set; }
        public string? Complaint_SubStatus { get; set; }
        public string? Complaint_Status { get; set; }
        public string? product { get; set; }
        public string? calltype { get; set; }
    }

}
