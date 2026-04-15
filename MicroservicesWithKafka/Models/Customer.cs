namespace MicroservicesWithKafka.Models
{
    public class Customer
    {
        // numeric(18, 0) fields map to long? (except Cust_ID which is the identity)
        public long Cust_ID { get; set; }
        public long? CUSTINTCODE { get; set; }
        public long? AMC_SalesID { get; set; }

        public string? SALUTATION { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? MIDDLENAME { get; set; }
        public string? LASTNAME { get; set; }

        public string? ADDRESS1 { get; set; }
        public string? ADDRESS2 { get; set; }
        public string? ADDRESS3 { get; set; }
        public string? StdCode { get; set; }

        public long? City_ID { get; set; }
        public string? CITY { get; set; }
        public long? PINCODE { get; set; } // numeric(18,0)

        public string? HMPH { get; set; }
        public string? WKPH { get; set; }
        public string? MOBILE { get; set; }
        public string? EMAIL { get; set; }

        public DateTime? DOB { get; set; }
        public DateTime? TDATE { get; set; }
        public DateTime? TTIME { get; set; }
        public string? ENTERBY { get; set; }
        public string? AREA_CD_HOME { get; set; }
        public string? AREA_CD_OFFICE { get; set; }
        public string? STATE { get; set; }
        public string? landmark { get; set; }
        public string? product { get; set; }
        public string? addedbydlr { get; set; }
        public string? isuploaded { get; set; } // varchar(2)
        public string? RCOMP { get; set; } // varchar(2)
        public string? asc_name { get; set; }
        public DateTime? UploadedDateTime { get; set; }

        public long? HW_ID { get; set; } // numeric(18,0)
        public string? Locality { get; set; }
        public long? Locality_ID { get; set; } // numeric(18,0)

        // bit fields map to bool?
        public int? TransferdCRM { get; set; }
        public int? FromWeb { get; set; }

        // Stored Procedure Output
        public string? StatusMsg { get; set; }
        public int? StatusCode { get; set; }
    }

}
