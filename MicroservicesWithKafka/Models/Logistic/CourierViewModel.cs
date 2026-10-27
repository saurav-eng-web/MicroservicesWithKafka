namespace MicroservicesWithKafka.Models.Logistic
{
    /// <summary>CourierMaster row: CourierName = courier code, CourierName1 = courier name (legacy CourierAdd.aspx).</summary>
    public class CourierViewModel
    {
        public string CourierCode { get; set; } = string.Empty;
        public string CourierName { get; set; } = string.Empty;
    }
}
