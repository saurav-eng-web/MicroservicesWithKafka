using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MicroservicesWithKafka.Models
{
    public class CallRegisterModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Stored Procedure parameters mapping
        public string? SALUTATION { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
        public string? HMPH { get; set; }           // Home Phone
        public string? WKPH { get; set; }           // Work Phone
        public string? STDCODE { get; set; }
        public string? PINCODE { get; set; }
        public string? MOBILE { get; set; }
        public string? ADDRESS1 { get; set; }
        public string? ADDRESS2 { get; set; }
        public string? CallTransferBY { get; set; } // enterby for update purpose
        public long? CITY_ID { get; set; }
        public long? LOCALITY_ID { get; set; }
        public string? LANDMARK { get; set; }
        public string? CALL_CATEGORY { get; set; }
        public long? CALL_TYPEID { get; set; }
        public long? CALLBY_ID { get; set; }
        public long? WARRANTYID { get; set; }
        public string? CALL_TYPE { get; set; }
        public string? CALL_BY { get; set; }
        public string? dlcity { get; set; }
        public string? DEALER { get; set; }
        public string? Dealer_ID { get; set; }
        public string? WARRANTY { get; set; }
        public string? PRODUCT { get; set; }
        public long? PRODUCT_ID { get; set; }
        public long? MODEL_ID { get; set; }
        public DateTime? PURCHASE_DATE { get; set; }
        public string? SERIAL_NO { get; set; }
        public string? COMPLAINT { get; set; }
        public long? SF_ID { get; set; }
        public DateTime? PROMISE_DATE { get; set; }
        public string? PROMISE_TIME { get; set; }
        public string? CATEGORY { get; set; }
        public string? ENTERBY { get; set; }
        public string? COMPLAINT_STATUS { get; set; }
        public string? REMARKS { get; set; }
        public string? EMAIL { get; set; }
        public string? COMPLAINT_SUBSTATUS { get; set; }
        public long? EXCALATED { get; set; }
        public long? LEGAL { get; set; }
        public string? ENTEREDBY { get; set; }
        public long? ENTEREDBYID { get; set; }
        public string? IPADDRESS { get; set; }
        public string? CancellationPreStatus { get; set; }
        [BsonElement("Bid")]
        public int? Bid { get; set; }

        [BsonElement("RegionID")]
        public int? RegionID { get; set; }

        public int? ASMID { get; set; }

        // Extra optional fields for API logic
        public long? CallId { get; set; }
        public string? CallNumber { get; set; }
        public string? TICKET_NO { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Locality { get; set; }
        public string? ProductCategory { get; set; }
        public string? Brand { get; set; }
        public DateTime? POP { get; set; }
        public string? Model { get; set; }
        public string? FaultVoc { get; set; }
        public string? AvailableTime { get; set; }
        public string? Asc_Name { get; set; }
        public string? ASC_CODE { get; set; }
        public string? ImageUpload { get; set; }
        public int? UserId { get; set; }

        

    }
}
