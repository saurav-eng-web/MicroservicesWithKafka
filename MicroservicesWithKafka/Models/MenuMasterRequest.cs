namespace MicroservicesWithKafka.Models
{
    public class MenuMasterRequest
    {
        public string? Action { get; set; }
        public int? MenuID { get; set; }
        public string? Menu_Name { get; set; }
        public string? Hierarchy { get; set; }
        public int? DisplayOrder { get; set; }
        public string? Menu_Role { get; set; }
        public string? Navigation_Url { get; set; }
        public string? Icon { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
