

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroservicesWithKafka.Models
{
    public class ComplaintHistory
    {
        // Tbl_ComplaintHistory fields
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? TicketNo { get; set; }
        public string? ComplaintStatus { get; set; }
        public string? ComplaintSubStatus { get; set; }
        public DateTime? Tdate { get; set; }
        public string? EnteredBy { get; set; }
        public long? EnteredByID { get; set; }
   
    }
}
