namespace MicroservicesWithKafka.Models
{
    public class CallRequest
    {
        public string? CallID { get; set; }  // use Guid for MongoDB, identity in SQL
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Issue { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Add other common fields
    }
}
