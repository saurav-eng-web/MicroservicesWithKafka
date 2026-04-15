using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class StateMasterRepository : IStateMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public StateMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private SqlCommand BuildCommand(SqlConnection connection, string action, StateMaster state)
        {
            var command = new SqlCommand("SP_CRUD_StateMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", action);
            command.Parameters.AddWithValue("@State_id", (object?)state.State_id ?? DBNull.Value);
            command.Parameters.AddWithValue("@State_name", (object?)state.State_name ?? DBNull.Value);
            command.Parameters.AddWithValue("@StateCode", (object?)state.StateCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@State_Status", (object?)state.State_Status ?? DBNull.Value);

            return command;
        }

        public async Task<StateMaster> CreateStateAsync(StateMaster state)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "insert", state);
            var result = await command.ExecuteScalarAsync();
            state.State_id = Convert.ToDecimal(result);

            return state;
        }

        public async Task<bool> UpdateStateAsync(StateMaster state)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "update", state);
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteStateAsync(decimal stateId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_StateMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "delete");
            command.Parameters.AddWithValue("@State_id", stateId);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<StateMaster?> GetStateByIdAsync(decimal stateId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_StateMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "getbyid");
            command.Parameters.AddWithValue("@State_id", stateId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToState(reader);
            }

            return null;
        }

        public async Task<List<StateMaster>> GetAllStatesAsync()
        {
            var states = new List<StateMaster>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_StateMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "get");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                states.Add(MapToState(reader));
            }

            return states;
        }

        private StateMaster MapToState(SqlDataReader reader)
        {
            return new StateMaster
            {
                State_id = reader["State_id"] as decimal?,
                State_name = reader["State_name"]?.ToString(),
                StateCode = reader["StateCode"]?.ToString(),
                State_Status = reader["State_Status"] as bool?
            };
        }
    }
}
