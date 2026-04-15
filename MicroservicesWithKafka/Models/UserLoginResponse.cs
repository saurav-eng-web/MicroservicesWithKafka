namespace MicroservicesWithKafka.Models
{
    public class UserLoginResponse
    {
        public int ReturnCode { get; set; }           // 1 = Success, -1 = Failed
        public int? UserID { get; set; }              // Nullable because it may not be returned if login fails
        public int? RoleID { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }          // From RM_Name in RoleMaster table
        public string Message { get; set; }           // Success or failure message

        public int? SF_ID { get; set; }

        public int? Cat_MapID { get; set; }

        public int? ASMID { get; set; }

        public int? Branch_ID { get; set; }
    }

}
