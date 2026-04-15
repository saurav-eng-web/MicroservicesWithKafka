using System;

namespace MicroservicesWithKafka.Models
{
    public class CallCreationUploader
    {
        public int Id { get; set; }

        // Customer Details
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public bool? MobileVal { get; set; }
        public string? Mobile { get; set; }
        public string? Alternate_Phone { get; set; }
        public string? Permanent_Address { get; set; }
        public string? Landmark { get; set; }
        public bool? PincodeVal { get; set; }
        public long? Pincode { get; set; }

        // Product Details
        public string? Title { get; set; }
        public string? Serial_No { get; set; }
        public string? Model { get; set; }
        public int? ModelVal { get; set; }

        // Call Details
        public string? CallBy { get; set; }
        public string? CallType { get; set; }
        public DateTime? Purchase_Date { get; set; }

        // Audit Fields
        public DateTime? CreatedDateTime { get; set; }
        public bool? Isprocess1 { get; set; }
        public bool? Isprocess { get; set; }
        public string? CreatedBy { get; set; }
        public string? IpAddress { get; set; }
    }
}
