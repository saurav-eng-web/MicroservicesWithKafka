using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class CallTypeRepository : ICallTypeRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CallTypeRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<List<CallTypeMaster>> GetCallTypesBySubcategoryAsync(string subCategory)
        {
            var callTypes = new List<CallTypeMaster>();

            if (string.IsNullOrEmpty(subCategory))
                return callTypes;

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(@"
                SELECT DISTINCT CallTypeId AS CallType_ID, CallType AS CallType_Name, Remarks AS CallType_Active
                FROM SubCategoryMAppingWithCallType 
                
                WHERE Remarks = 'Yes' AND SubCategory = @SubCategory", connection);

            command.Parameters.AddWithValue("@SubCategory", subCategory);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                callTypes.Add(new CallTypeMaster
                {
                    CallType_ID = (int)reader["CallType_ID"],
                    CallType_Name = reader["CallType_Name"]?.ToString(),
                    CallType_Active = reader["CallType_Active"]?.ToString(),
                    
                });
            }

            return callTypes;
        }
    }
}
