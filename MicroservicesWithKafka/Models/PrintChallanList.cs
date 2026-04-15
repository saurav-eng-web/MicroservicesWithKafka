namespace MicroservicesWithKafka.Models
{
    public class PrintChallanList
    {
        // ✅ Input fields
        public string? SfId { get; set; }
        public string? Type { get; set; }
        public string? Month { get; set; }
        public string? Year { get; set; }

        // ✅ Output fields
        public string? Id { get; set; }
        public string? PoNumber { get; set; }
        public string? SfName { get; set; }
        public string? Date { get; set; }
        public string? Status { get; set; }

        public string? docket { get; set; }

        // ✅ Optional message (for response status or errors)
        public string? Message { get; set; }
    }
}
