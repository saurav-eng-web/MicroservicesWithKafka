using Microsoft.Data.SqlClient;
using MicroservicesWithKafka.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MicroservicesWithKafka.Services;

namespace MicroservicesWithKafka.Repository
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ReminderRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<long> InsertAsync(Reminder model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Reminder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "INSERT");
            cmd.Parameters.AddWithValue("@Remind_Bid", model.Remind_Bid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Sfid", model.Remind_Sfid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Custintcode", model.Remind_Custintcode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_TicketNo", model.Remind_TicketNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Remark", model.Remind_Remark ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Status", model.Remind_Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Enterby", model.Remind_Enterby ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Trap", model.Remind_Trap ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Ticket_No", model.Ticket_No ?? (object)DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<int> UpdateAsync(Reminder model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Reminder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "UPDATE");
            cmd.Parameters.AddWithValue("@Remind_ID", model.Remind_ID);
            cmd.Parameters.AddWithValue("@Remind_Bid", model.Remind_Bid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Sfid", model.Remind_Sfid ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Custintcode", model.Remind_Custintcode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_TicketNo", model.Remind_TicketNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Remark", model.Remind_Remark ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Status", model.Remind_Status ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Enterby", model.Remind_Enterby ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Remind_Trap", model.Remind_Trap ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Ticket_No", model.Ticket_No ?? (object)DBNull.Value);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Reminder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "DELETE");
            cmd.Parameters.AddWithValue("@Remind_ID", id);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Reminder?> GetByIdAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Reminder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@Remind_ID", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToReminder(reader);
            }
            return null;
        }

        public async Task<IEnumerable<Reminder>> GetAllAsync()
        {
            var list = new List<Reminder>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Reminder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "GET_ALL");

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToReminder(reader));
            }
            return list;
        }

        public async Task<IEnumerable<Reminder>> GetByTicketAsync(string ticketNo)
        {
            var reminders = new List<Reminder>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_CRUD_Reminder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Type", "GET_BY_TicketNo");
            cmd.Parameters.AddWithValue("@Ticket_No", ticketNo);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())   // ✅ loop instead of single read
            {
                reminders.Add(MapToReminder(reader));
            }

            return reminders;
        }


        private Reminder MapToReminder(SqlDataReader reader)
        {
            return new Reminder
            {
                Remind_ID = Convert.ToInt64(reader["Remind_ID"]),
                Remind_Bid = reader["Remind_Bid"] as long?,
                Remind_Sfid = reader["Remind_Sfid"] as long?,
                Remind_Custintcode = reader["Remind_Custintcode"] as long?,
                Remind_TicketNo = reader["Remind_TicketNo"] as long?,
                Remind_Remark = reader["Remind_Remark"]?.ToString(),
                Remind_date = Convert.ToDateTime(reader["Remind_date"]),
                Remind_Status = reader["Remind_Status"]?.ToString(),
                Remind_Enterby = reader["Remind_Enterby"]?.ToString(),
                Remind_Trap = reader["Remind_Trap"]?.ToString(),
                Ticket_No = reader["Ticket_No"]?.ToString()
            };
        }
    }
}

