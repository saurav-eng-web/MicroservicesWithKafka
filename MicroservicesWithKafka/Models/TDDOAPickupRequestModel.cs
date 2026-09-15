namespace MicroservicesWithKafka.Models
{
    public class TDDOAPickupRequestModel
    {
        public string SerialNumber { get; set; }   // required for INSERT
        public string Action { get; set; }         // INSERT / DISTSELECT / LOGSELECT
    }
}
