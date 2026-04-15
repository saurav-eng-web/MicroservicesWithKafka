using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ModelVariantRepository : IModelVariantRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ModelVariantRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<ModelVariant> CreateAsync(ModelVariant variant)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelVariant_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "INSERT");
            AddParameters(command, variant);

            await command.ExecuteNonQueryAsync();
            return variant;
        }

        public async Task<bool> UpdateAsync(ModelVariant variant)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelVariant_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "UPDATE");
            command.Parameters.AddWithValue("@VariantID", variant.VariantID ?? (object)DBNull.Value);
            AddParameters(command, variant);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(decimal variantId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelVariant_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "DELETE");
            command.Parameters.AddWithValue("@VariantID", variantId);

            int rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<ModelVariant> GetByIdAsync(decimal variantId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelVariant_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "SELECT");
            command.Parameters.AddWithValue("@VariantID", variantId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReader(reader);
            }

            return null;
        }

        public async Task<List<ModelVariant>> GetAllAsync()
        {
            var variants = new List<ModelVariant>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelVariant_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "SELECT");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                variants.Add(MapReader(reader));
            }

            return variants;
        }

        private void AddParameters(SqlCommand command, ModelVariant variant)
        {
            command.Parameters.AddWithValue("@ModelID", variant.ModelID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Price", variant.Price ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@StockQuantity", variant.StockQuantity ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", variant.IsActive ?? true);
            command.Parameters.AddWithValue("@CreatedDate", variant.CreatedDate ?? DateTime.Now);
            command.Parameters.AddWithValue("@Attribute1", variant.Attribute1 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Attribute2", variant.Attribute2 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Attribute3", variant.Attribute3 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Attribute4", variant.Attribute4 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Attribute5", variant.Attribute5 ?? (object)DBNull.Value);
        }

        private ModelVariant MapReader(SqlDataReader reader)
        {
            return new ModelVariant
            {
                VariantID = reader["VariantID"] as decimal?,
                ModelID = reader["ModelID"] as decimal?,
                Price = reader["Price"] as decimal?,
                StockQuantity = reader["StockQuantity"] as int?,
                IsActive = reader["IsActive"] as bool?,
                CreatedDate = reader["CreatedDate"] as DateTime?,
                Attribute1 = reader["Attribute1"]?.ToString(),
                Attribute2 = reader["Attribute2"]?.ToString(),
                Attribute3 = reader["Attribute3"]?.ToString(),
                Attribute4 = reader["Attribute4"]?.ToString(),
                Attribute5 = reader["Attribute5"]?.ToString()
            };
        }
    }
}
