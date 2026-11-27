namespace MicroservicesWithKafka.Models
{
    public class ForwardDashboardSummaryViewModel
    {
        public int PickupAwaited { get; set; }
        public int InTransit { get; set; }
        public int Delivered { get; set; }
        public int ReceiptPending { get; set; }
    }

}
