namespace MicroservicesWithKafka.Models
{
    public class LocalityMaster
    {
        public decimal? Locality_Id { get; set; }
        public decimal? Locality_RegionID { get; set; }
        public decimal? Locality_BranchID { get; set; }
        public decimal? Locality_City { get; set; }
        public string? Locality_Name { get; set; }
        public string? Pincode { get; set; }
        public bool? Locality_Status { get; set; }
        public string? locality_c_u { get; set; }
        public decimal? Locality_State { get; set; }
        public decimal? ResolutionTATID { get; set; }
        public decimal? Distance { get; set; }
        public string? Installation_Demo_TAT { get; set; }
        public string? ServiceCall_Visit_TAT { get; set; }
        public string? ServiceCall_Repair_TAT { get; set; }
        public decimal? Installation_Demo_TAT_Num { get; set; }
        public decimal? ServiceCall_Repair_TAT_Num { get; set; }

        public string? State_Name { get; set; }
        public string? City_Name { get; set; }
    }
}
