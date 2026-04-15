using System;

namespace MicroservicesWithKafka.Models
{
    public class WarrantyRegistration
    {
        public long ID { get; set; }
        public string? CustomerName { get; set; }
        public long? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public long? Pincode { get; set; }
        public string? City { get; set; }
        public long? CityID { get; set; }
        public string? Locality { get; set; }
        public long? LocalityID { get; set; }
        public string? SerialNumber { get; set; }
        public string? Product { get; set; }
        public long? ProductID { get; set; }
        public string? Model { get; set; }
        public long? ModelID { get; set; }
        public DateTime? Tdate { get; set; }
        public string? EnterBy { get; set; }
        public DateTime? DOP { get; set; }
        public string? PurchaseFromRetailer { get; set; }
        public string? Invoice { get; set; }
        public long? BrandID { get; set; }
        public string? Brand { get; set; }
        public string? LastName { get; set; }
        public string? LandMark { get; set; }
        public string? Warranty { get; set; }
    }
}
