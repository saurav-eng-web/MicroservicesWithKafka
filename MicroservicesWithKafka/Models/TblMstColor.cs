namespace MicroservicesWithKafka.Models
{
    public class TblMstColor
    {
        public decimal? Col_ID { get; set; } // numeric(18, 0)
        public string? Col_Desc { get; set; } // nvarchar(250)
        public bool? Col_Status { get; set; } // bit
    }
}