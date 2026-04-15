using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ComplaintHistoryRepository : IComplaintHistoryRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ComplaintHistoryRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

      

        public async Task<long> InsertAsync(ComplaintHistory model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ComplaintHistory", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "INSERT");
            command.Parameters.AddWithValue("@Ticket_No", model.TicketNo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Complaint_status", model.ComplaintStatus ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Complaint_subStatus", model.ComplaintSubStatus ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Tdate", model.Tdate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredBy", model.EnteredBy ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredByID", model.EnteredByID ?? (object)DBNull.Value);
            

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(ComplaintHistory model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ComplaintHistory", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "UPDATE");
            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@Ticket_No", model.TicketNo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Complaint_status", model.ComplaintStatus ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Complaint_subStatus", model.ComplaintSubStatus ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Tdate", model.Tdate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredBy", model.EnteredBy ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnteredByID", model.EnteredByID ?? (object)DBNull.Value);

            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ComplaintHistory", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "DELETE");
            command.Parameters.AddWithValue("@Id", id);

            var result = await command.ExecuteReaderAsync();
            if (await result.ReadAsync())
            {
                int rowsAffected = Convert.ToInt32(result["RowsAffected"]);
                return rowsAffected > 0;
            }
            return false;
        }

        public async Task<List<ComplaintHistory>> GetAllAsync()
        {
            var list = new List<ComplaintHistory>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ComplaintHistory", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Type", "GET_ALL");

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new ComplaintHistory
                {
                    //Id = Convert.ToInt64(reader["Id"]),
                    TicketNo = reader["Ticket_No"]?.ToString(),
                    ComplaintStatus = reader["Complaint_status"]?.ToString(),
                    ComplaintSubStatus = reader["Complaint_subStatus"]?.ToString(),
                    Tdate = reader["Tdate"] as DateTime?,
                    EnteredBy = reader["EnteredBy"]?.ToString(),
                    EnteredByID = reader["EnteredByID"] == DBNull.Value ? null : Convert.ToInt64(reader["EnteredByID"])
                });
            }

            return list;
        }

        public async Task<ComplaintHistory?> GetByIdAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ComplaintHistory", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "GET_BY_ID");
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ComplaintHistory
                {
                    //Id = Convert.ToInt64(reader["Id"]),
                    TicketNo = reader["Ticket_No"]?.ToString(),
                    ComplaintStatus = reader["Complaint_status"]?.ToString(),
                    ComplaintSubStatus = reader["Complaint_subStatus"]?.ToString(),
                    Tdate = reader["Tdate"] as DateTime?,
                    EnteredBy = reader["EnteredBy"]?.ToString(),
                    EnteredByID = reader["EnteredByID"] == DBNull.Value ? null : Convert.ToInt64(reader["EnteredByID"])
                };
            }

            return null;
        }
        public async Task<List<ComplaintHistory>> GetAllByTicketNoAsync(string ticketNo)
        {
            var list = new List<ComplaintHistory>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ComplaintHistory", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "GET_ALL_BY_TICKET_NO");
            command.Parameters.AddWithValue("@Ticket_No", ticketNo);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new ComplaintHistory
                {
                    //Id = Convert.ToInt64(reader["Id"]),
                    TicketNo = reader["Ticket_No"]?.ToString(),
                    ComplaintStatus = reader["Complaint_status"]?.ToString(),
                    ComplaintSubStatus = reader["Complaint_subStatus"]?.ToString(),
                    Tdate = reader["Tdate"] as DateTime?,
                    EnteredBy = reader["EnteredBy"]?.ToString(),
                    EnteredByID = reader["EnteredByID"] == DBNull.Value ? null : Convert.ToInt64(reader["EnteredByID"])

                });
            }

            return list;
        }


    }
}
