using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroservicesWithKafka.Models
{
    public class DashboardBucket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("bucket")]
        public string Bucket { get; set; }

        [BsonElement("complaintStatus")]
        public string? ComplaintStatus { get; set; }

        [BsonElement("complaintSubStatus")]
        public string? ComplaintSubStatus { get; set; }

        [BsonElement("role")]
        public int Role { get; set; }

        [BsonElement("navigatingUrl")]
        public string? NavigatingUrl { get; set; }

        [BsonElement("CNT")]
        public int CNT { get; set; }
    }
}
