namespace MicroservicesWithKafka.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Mobno { get; set; }
        public string Mail_ID { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UPassword { get; set; }

        public string Status { get; set; }
        public int RoleID { get; set; }
        public int Branch_ID { get; set; }
        public bool Branch_can_RegisterCall { get; set; }

        //public int? SF_ID { get; set; }

        //public int? Cat_MapID { get; set; }

        //public int? ASMID { get; set; }
    }

}
