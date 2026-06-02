namespace MicroservicesWithKafka.Models
{
    public class UpdateUserResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
