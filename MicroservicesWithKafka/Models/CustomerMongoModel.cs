using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MicroservicesWithKafka.Models
{
    public class CustomerMongoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? SALUTATION { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
        public string? MOBILE { get; set; }
        public string? EMAIL { get; set; }
        public string? ADDRESS1 { get; set; }
        public string? ADDRESS2 { get; set; }
        public int? CITY_ID { get; set; }
        public string? PINCODE { get; set; }
        public string? LANDMARK { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? TICKET_NO { get; set; }
    }
}
