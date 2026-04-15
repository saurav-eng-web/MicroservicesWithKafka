using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class MstColorRepository : IMstColorRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public MstColorRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<TblMstColor> CreateColorAsync(TblMstColor color)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_MstColor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "insert");
            command.Parameters.AddWithValue("@Col_Desc", (object?)color.Col_Desc ?? DBNull.Value);
            command.Parameters.AddWithValue("@Col_Status", color.Col_Status ?? false);

            var result = await command.ExecuteScalarAsync();
            color.Col_ID = Convert.ToDecimal(result);

            return color;
        }

        public async Task<bool> UpdateColorAsync(TblMstColor color)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_MstColor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "update");
            command.Parameters.AddWithValue("@Col_ID", color.Col_ID);
            command.Parameters.AddWithValue("@Col_Desc", (object?)color.Col_Desc ?? DBNull.Value);
            command.Parameters.AddWithValue("@Col_Status", color.Col_Status ?? false);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                return rows > 0;
            }

            return false;
        }
        public async Task<bool> DeleteColorAsync(decimal colId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_MstColor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "delete");
            command.Parameters.AddWithValue("@Col_ID", colId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                return rows > 0;
            }

            return false;
        }
        public async Task<TblMstColor?> GetColorByIdAsync(decimal colId)
        {
            TblMstColor? color = null;

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_MstColor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "getbyid");
            command.Parameters.AddWithValue("@Col_ID", colId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                color = new TblMstColor
                {
                    Col_ID = (decimal)reader["Col_ID"],
                    Col_Desc = reader["Col_Desc"]?.ToString(),
                    Col_Status = reader["Col_Status"] as bool?
                };
            }

            return color;
        }

        public async Task<List<TblMstColor>> GetAllColorsAsync()
        {
            var colors = new List<TblMstColor>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_MstColor", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "get");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                colors.Add(new TblMstColor
                {
                    Col_ID = (decimal)reader["Col_ID"],
                    Col_Desc = reader["Col_Desc"]?.ToString(),
                    Col_Status = reader["Col_Status"] as bool?
                });
            }

            return colors;
        }
    }
}
