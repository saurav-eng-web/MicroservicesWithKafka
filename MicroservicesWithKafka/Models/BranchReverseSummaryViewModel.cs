namespace MicroservicesWithKafka.Models
{
    public class BranchReverseSummaryViewModel
    {
        public int PickupAwaited { get; set; }
        public int InTransit { get; set; }
        public int Delivered { get; set; }
        public int ChallanToBeCancel { get; set; }
    }
}
