namespace MicroservicesWithKafka.Models
{
    public class TDDOAPickupRequestModel
    {
        public string SerialNumber { get; set; }   // required for INSERT

        // INSERT / DISTSELECT / distwisecount / distwisecount1 / LOGSELECT / LOGISTICUPLOAD
        public string Action { get; set; }
    }
}
