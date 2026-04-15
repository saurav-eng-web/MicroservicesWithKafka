namespace MicroservicesWithKafka.Models.Requests
{
    public class DropdownRequest
    {
        // ✅ Input parameters (from API body)
        public string? Type { get; set; }
        public string? DispatchType { get; set; }
    }
}
