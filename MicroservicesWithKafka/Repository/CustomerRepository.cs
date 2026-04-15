using Dapper;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Data;

namespace MicroservicesWithKafka.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CustomerRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private const string StoredProcedureName = "SP_CRUD_CUSTOMER";

        // Helper method to populate common parameters
        private DynamicParameters GetParameters(string action, Customer? model = null, long? id = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Action", action);

            if (model != null)
            {
                parameters.Add("@Cust_ID", model.Cust_ID > 0 ? model.Cust_ID : (long?)null);
                parameters.Add("@CUSTINTCODE", model.CUSTINTCODE);
                parameters.Add("@AMC_SalesID", model.AMC_SalesID);
                parameters.Add("@SALUTATION", model.SALUTATION);
                parameters.Add("@FIRSTNAME", model.FIRSTNAME);
                parameters.Add("@MIDDLENAME", model.MIDDLENAME);
                parameters.Add("@LASTNAME", model.LASTNAME);
                parameters.Add("@ADDRESS1", model.ADDRESS1);
                parameters.Add("@ADDRESS2", model.ADDRESS2);
                parameters.Add("@ADDRESS3", model.ADDRESS3);
                parameters.Add("@StdCode", model.StdCode);
                parameters.Add("@City_ID", model.City_ID);
                parameters.Add("@CITY", model.CITY);
                parameters.Add("@PINCODE", model.PINCODE);
                parameters.Add("@HMPH", model.HMPH);
                parameters.Add("@WKPH", model.WKPH);
                parameters.Add("@MOBILE", model.MOBILE);
                parameters.Add("@EMAIL", model.EMAIL);
                parameters.Add("@DOB", model.DOB);
                parameters.Add("@TDATE", model.TDATE);
                parameters.Add("@TTIME", model.TTIME);
                parameters.Add("@ENTERBY", model.ENTERBY);
                parameters.Add("@AREA_CD_HOME", model.AREA_CD_HOME);
                parameters.Add("@AREA_CD_OFFICE", model.AREA_CD_OFFICE);
                parameters.Add("@STATE", model.STATE);
                parameters.Add("@landmark", model.landmark);
                parameters.Add("@product", model.product);
                parameters.Add("@addedbydlr", model.addedbydlr);
                parameters.Add("@isuploaded", model.isuploaded);
                parameters.Add("@RCOMP", model.RCOMP);
                parameters.Add("@asc_name", model.asc_name);
                parameters.Add("@UploadedDateTime", model.UploadedDateTime);
                parameters.Add("@HW_ID", model.HW_ID);
                parameters.Add("@Locality", model.Locality);
                parameters.Add("@Locality_ID", model.Locality_ID);
                parameters.Add("@TransferdCRM", model.TransferdCRM); // Now int?
                parameters.Add("@FromWeb", model.FromWeb);           // Now int?
            }
            else if (id.HasValue)
            {
                parameters.Add("@Cust_ID", id.Value);
            }

            return parameters;
        }


        // --- CREATE ---
        public async Task<Customer> CreateAsync(Customer model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var parameters = GetParameters("INSERT", model);

            // SP returns Cust_ID, StatusMsg, StatusCode
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                StoredProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            model.Cust_ID = (long?)result?.Cust_ID ?? 0;
            model.StatusMsg = result?.StatusMsg;
            model.StatusCode = result?.StatusCode;

            return model;
        }

        // --- UPDATE ---
        public async Task<Customer> UpdateAsync(Customer model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var parameters = GetParameters("UPDATE", model);

            // SP returns StatusMsg, StatusCode
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                StoredProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            model.StatusMsg = result?.StatusMsg;
            model.StatusCode = result?.StatusCode;

            return model;
        }

        // --- DELETE ---
        public async Task<Customer> DeleteAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var parameters = GetParameters("DELETE", id: id);

            // SP returns StatusMsg, StatusCode
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                StoredProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return new Customer
            {
                Cust_ID = id,
                StatusMsg = result?.StatusMsg,
                StatusCode = result?.StatusCode
            };
        }

        // --- GET ALL ---
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            using var connection = _sqlConnectionService.GetConnection();
            var result = await connection.QueryAsync<Customer>(
                StoredProcedureName,
                GetParameters("SELECT"),
                commandType: CommandType.StoredProcedure
            );
            return result.ToList();
        }

        // --- GET BY ID ---
        public async Task<Customer?> GetByIdAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            var result = await connection.QueryFirstOrDefaultAsync<Customer>(
                StoredProcedureName,
                GetParameters("SELECT", id: id),
                commandType: CommandType.StoredProcedure
            );
            return result;
        }
    }
}
