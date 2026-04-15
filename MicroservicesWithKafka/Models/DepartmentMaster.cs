namespace MicroservicesWithKafka.Models
{
    public class DepartmentMaster
    {
        public long? DepartmentID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
