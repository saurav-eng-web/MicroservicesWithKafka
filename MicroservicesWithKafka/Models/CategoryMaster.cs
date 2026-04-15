namespace MicroservicesWithKafka.Models
{
    public class CategoryMaster
    {
        public decimal? Cat_ID { get; set; }
        public string Cat_Name { get; set; }
        public decimal Cat_BrandID { get; set; }
        public string Cat_Code { get; set; }
        public bool cat_Status { get; set; }
        public string Cat_FullName { get; set; }
        public bool ForB2B { get; set; }

    }
}
