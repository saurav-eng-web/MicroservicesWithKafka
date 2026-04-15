using Dapper;
using System.Data;
using MicroservicesWithKafka.Models.Requests;
using MicroservicesWithKafka.Models.Responses;
using MicroservicesWithKafka.Services;

namespace MicroservicesWithKafka.Repository
{
    public class DropdownMasterRepository : IDropdownMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public DropdownMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<IEnumerable<DropdownResponse>> GetDropdownAsync(DropdownRequest request)
        {
            try
            {
                using var connection = _sqlConnectionService.GetConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@type", request.Type);
                parameters.Add("@Dispatchtype", request.DispatchType);

                var result = await connection.QueryAsync<DropdownResponse>(
                    "SP_CRUD_Dropdownmaster",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                return new List<DropdownResponse>
                {
                    new DropdownResponse { Message = $"Error: {ex.Message}" }
                };
            }
        }
    }
}
