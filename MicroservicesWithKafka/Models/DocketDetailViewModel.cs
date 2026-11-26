using System.Collections.Generic;

namespace MicroservicesWithKafka.Models
{
    public class DocketDetailViewModel
    {
        public string? ChallanNo { get; set; }
        public string? Docket { get; set; }
        public string? Courier { get; set; }
        public string? TypeOfChallan { get; set; }
        public string? CourierWeight { get; set; }
        public string? Dimensions { get; set; }
        public string? PickupBoy { get; set; }
        public string? PickupMobile { get; set; }
        public string? DocketStatus { get; set; }
        public string? PickupRemark { get; set; }
        public List<DocketHistoryViewModel> History { get; set; } = new List<DocketHistoryViewModel>();
    }
}
