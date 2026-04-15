namespace MicroservicesWithKafka.Models.Responses
{
    public class DropdownResponse
    {
        // ✅ Data returned from DB
        public string? Key { get; set; }
        public string? Value { get; set; }

        // ✅ Optional message
        public string? Message { get; set; }
    }
}
