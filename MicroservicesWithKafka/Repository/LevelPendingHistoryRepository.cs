using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class LevelPendingHistoryRepository : ILevelPendingHistoryRepository
    {
        private readonly SQLConnectionService _connectionService;

        public LevelPendingHistoryRepository(SQLConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<int> CreateAsync(LevelPendingHistory entity)
        {
            using var conn = _connectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_LevelPendingHistory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@type", "INSERT");
            cmd.Parameters.AddWithValue("@TicketNo", entity.Ticket_No ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TicketKey", entity.Ticket_Key ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PendingAt", entity.PendingAt ?? (object)DBNull.Value);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<IEnumerable<LevelPendingHistory>> GetAllAsync()
        {
            var list = new List<LevelPendingHistory>();
            using var conn = _connectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_LevelPendingHistory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@type", "GET_ALL");

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public async Task<LevelPendingHistory> GetByIdAsync(int id)
        {
            using var conn = _connectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_LevelPendingHistory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@type", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@ID", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Map(reader);
            }
            return null;
        }

        public async Task<IEnumerable<LevelPendingHistory>> GetByTicketNoAsync(string ticketNo)
        {
            var list = new List<LevelPendingHistory>();
            using var conn = _connectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_LevelPendingHistory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@type", "GET_BY_TICKET");
            cmd.Parameters.AddWithValue("@TicketNo", ticketNo);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(Map(reader));
            }
            return list;
        }

        public async Task UpdateAsync(LevelPendingHistory entity)
        {
            using var conn = _connectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_LevelPendingHistory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@type", "UPDATE");
            cmd.Parameters.AddWithValue("@ID", entity.ID);
            cmd.Parameters.AddWithValue("@TicketNo", entity.Ticket_No ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TicketKey", entity.Ticket_Key ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PendingAt", entity.PendingAt ?? (object)DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _connectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_LevelPendingHistory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@type", "DELETE");
            cmd.Parameters.AddWithValue("@ID", id);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        private LevelPendingHistory Map(SqlDataReader reader)
        {
            return new LevelPendingHistory
            {
                ID = Convert.ToInt32(reader["ID"]),
                Ticket_No = reader["Ticket_No"].ToString(),
                Ticket_Key = reader["Ticket_Key"].ToString(),
                PendingAt = reader["PendingAt"].ToString()
            };
        }
    }
}
