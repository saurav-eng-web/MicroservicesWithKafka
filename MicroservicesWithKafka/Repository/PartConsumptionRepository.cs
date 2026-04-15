using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class PartConsumptionRepository : IPartConsumptionRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public PartConsumptionRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        // 🔹 Get by Ticket No
        public async Task<IEnumerable<PartConsumption>> GetByTicketNoAsync(string ticketNo)
        {
            var results = new List<PartConsumption>();
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_PartConsumption", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TicketNo", ticketNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@type", "GET_BY_TICKET");

            conn.Open();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapToModel(reader));
            }
            return results;
        }

        // 🔹 Get by ID
        public async Task<PartConsumption?> GetByIdAsync(int id)
        {
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_PartConsumption", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@PC_ID", id);
            cmd.Parameters.AddWithValue("@type", "GET_BY_ID");

            conn.Open();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToModel(reader);
            }
            return null;
        }

        // 🔹 Insert
        public async Task<int> InsertAsync(PartConsumption pc)
        {
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_PartConsumption", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(cmd, pc);
            cmd.Parameters.AddWithValue("@type", "INSERT");

            conn.Open();
            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }

        // 🔹 Update
        public async Task UpdateAsync(PartConsumption pc)
        {
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_PartConsumption", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(cmd, pc);
            cmd.Parameters.AddWithValue("@type", "UPDATE");

            conn.Open();
            await cmd.ExecuteNonQueryAsync();
        }

        // 🔹 Delete
        public async Task DeleteAsync(int id)
        {
            using var conn = _sqlConnectionService.GetConnection();
            using var cmd = new SqlCommand("SP_CRUD_PartConsumption", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@PC_ID", id);
            cmd.Parameters.AddWithValue("@type", "DELETE");

            conn.Open();
            await cmd.ExecuteNonQueryAsync();
        }

        // ✅ Add parameters dynamically
        private void AddParameters(SqlCommand cmd, PartConsumption pc)
        {
            cmd.Parameters.AddWithValue("@PC_ID", pc.PC_ID);
            cmd.Parameters.AddWithValue("@PC_JobNo", (object?)pc.PC_JobNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_Srno", (object?)pc.PC_Srno ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_PartCode", (object?)pc.PC_PartCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_PartDesc", (object?)pc.PC_PartDesc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_Date", (object?)pc.PC_Date ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_Rate", (object?)pc.PC_Rate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_Quantity", (object?)pc.PC_Quantity ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_Amount", (object?)pc.PC_Amount ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_SendDef", (object?)pc.PC_SendDef ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_DefPOID", (object?)pc.PC_DefPOID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PC_Consumed", (object?)pc.PC_Consumed ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TicketNo", (object?)pc.Ticket_no ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Scrapped", (object?)pc.Scrapped ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DefPartSerialNo", (object?)pc.DefPartSerialNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PackSlipNo", (object?)pc.PackSlipNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ISReturnable", (object?)pc.ISReturnable ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsReplaced", (object?)pc.IsReplaced ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ISShort", (object?)pc.ISShort ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsFreshFaulty", (object?)pc.IsFreshFaulty ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AXStatus", (object?)pc.AXStatus ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@InventoryType", (object?)pc.InventoryType ?? DBNull.Value);
        }

        // ✅ Map Reader to Model
        private PartConsumption MapToModel(SqlDataReader reader)
        {
            return new PartConsumption
            {
                PC_ID = Convert.ToInt32(reader["PC_ID"]),
                PC_JobNo = reader["PC_JobNo"] as int?,
                PC_Srno = reader["PC_Srno"] as int?,
                PC_PartCode = reader["PC_PartCode"]?.ToString(),
                PC_PartDesc = reader["PC_PartDesc"]?.ToString(),
                PC_Date = reader["PC_Date"] as DateTime?,
                PC_Rate = reader["PC_Rate"] as decimal?,
                PC_Quantity = reader["PC_Quantity"] as int?,
                PC_Amount = reader["PC_Amount"] as decimal?,
                PC_SendDef = reader["PC_SendDef"] as int?,
                PC_DefPOID = reader["PC_DefPOID"] as int?,
                PC_Consumed = reader["PC_Consumed"] as int?,
                Ticket_no = reader["Ticket_no"]?.ToString(),
                Scrapped = reader["Scrapped"] as int?,
                DefPartSerialNo = reader["DefPartSerialNo"]?.ToString(),
                PackSlipNo = reader["PackSlipNo"]?.ToString(),
                ISReturnable = reader["ISReturnable"] as bool?,
                IsReplaced = reader["IsReplaced"] as bool?,
                ISShort = reader["ISShort"] as bool?,
                IsFreshFaulty = reader["IsFreshFaulty"] as bool?,
                AXStatus = reader["AXStatus"] as int?,
                InventoryType = reader["InventoryType"]?.ToString()
            };
        }
    }
}
