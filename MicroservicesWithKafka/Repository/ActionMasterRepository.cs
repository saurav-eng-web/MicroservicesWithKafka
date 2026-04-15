using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ActionMasterRepository : IActionMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ActionMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        // ==================================================
        // ✅ SELECT (JSON RESULT FROM SP)
        // ==================================================
        public async Task<string> GetActionsAsync(ActionMaster filterModel)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand("SP_CRUD_ActionMaster", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Type", filterModel.Type ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Role", filterModel.Role ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Complaint_SubStatus", filterModel.Complaint_SubStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@COMPLAINT_STATUS", filterModel.Complaint_Status ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@product", filterModel.product ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@calltype", filterModel.calltype ?? (object)DBNull.Value);

                    var json = await cmd.ExecuteScalarAsync();

                    return json == null ? "{}" : json.ToString();
                }
            }
        }

        // ==================================================
        // INSERT / UPDATE / DELETE KEEP AS-IS (OPTIONAL)
        // ==================================================
        public async Task<int> InsertActionAsync(ActionMaster model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_ActionMaster", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Type", model.Type ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Action", model.Action ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Role", model.Role ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Complaint_SubStatus", model.Complaint_SubStatus ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@COMPLAINT_STATUS", model.Complaint_Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@product", model.product ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@calltype", model.calltype ?? (object)DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<string> UpdateActionAsync(ActionMaster model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_ActionMaster", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", model.ID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Type", model.Type ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Action", model.Action ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Role", model.Role ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Complaint_SubStatus", model.Complaint_SubStatus ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@COMPLAINT_STATUS", model.Complaint_Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@product", model.product ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@calltype", model.calltype ?? (object)DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "Updated";
        }

        public async Task<string> DeleteActionAsync(int id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_ActionMaster", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ID", id);
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "Deleted";
        }

        public Task<IEnumerable<ActionMaster>> GetAllActionsAsync()
        {
            throw new NotSupportedException("This SP returns JSON. Use GetActionsAsync(filter) instead.");
        }
    }
}
