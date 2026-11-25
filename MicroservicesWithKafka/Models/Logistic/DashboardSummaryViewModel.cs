using System.Collections.Generic;

namespace MicroservicesWithKafka.Models.Logistic
{
    public class DashboardSummaryViewModel
    {
        public string? DefectivePart { get; set; }
        public string? DefectiveDOA { get; set; }
        public string? GoodPart { get; set; }
        public string? GoodMachine { get; set; }
        public string? MismatchPart { get; set; }
        public string? MismatchMachine { get; set; }
        public int PickupAwaited { get; set; }
        public int InTransit { get; set; }
        public List<CourierAvailabilityViewModel> CourierAvailability { get; set; } = new List<CourierAvailabilityViewModel>();
    }
}
