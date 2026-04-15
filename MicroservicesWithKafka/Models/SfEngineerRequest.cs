namespace MicroservicesWithKafka.Models
{
    public class SfEngineerRequest
    {
        public string Action { get; set; }
        public long? Eng_ID { get; set; }
        public long? Eng_RegionID { get; set; }
        public long? Eng_BranchID { get; set; }
        public long? Eng_SFId { get; set; }
        public string? Eng_Name { get; set; }
        public long? Eng_Catid { get; set; }
        public string? Eng_MobNo { get; set; }
        public string? Eng_Status { get; set; }
        public long? Eng_CityId { get; set; }
        public long? Eng_LocalityID { get; set; }
        public string? Eng_TotExpY { get; set; }
        public string? Eng_TotExpM { get; set; }
        public DateTime? Eng_DOJ { get; set; }
        public long? Eng_GradeID { get; set; }
        public string? Eng_ICard { get; set; }
        public string? IMEINo { get; set; }
        public string? SWcode { get; set; }
        public string? HighestQualification { get; set; }
        public string? TechnicalQualification { get; set; }
    }

}
