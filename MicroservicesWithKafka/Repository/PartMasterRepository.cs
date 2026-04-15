using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class PartMasterRepository : IPartMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public PartMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<PartMaster> CreatePartAsync(PartMaster part)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_PartMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "insert");

            AddParameters(command, part);

            var result = await command.ExecuteScalarAsync();
            part.PartID = Convert.ToDecimal(result);

            return part;
        }

        public async Task<bool> UpdatePartAsync(PartMaster part)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_PartMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "update");
            AddParameters(command, part);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                return rows > 0;
            }

            return false;
        }

        public async Task<bool> DeletePartAsync(decimal partId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_PartMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "delete");
            command.Parameters.AddWithValue("@PartID", partId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync() && reader["RowsAffected"] != DBNull.Value)
            {
                int rows = Convert.ToInt32(reader["RowsAffected"]);
                return rows > 0;
            }

            return false;
        }
        public async Task<PartMaster?> GetPartByIdAsync(decimal partId)
        {
            PartMaster? part = null;

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_PartMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "getbyid");
            command.Parameters.AddWithValue("@PartID", partId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                part = MapToPartMaster(reader);
            }

            return part;
        }

        public async Task<List<PartMaster>> GetAllPartsAsync()
        {
            var parts = new List<PartMaster>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_PartMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "get");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                parts.Add(MapToPartMaster(reader));
            }

            return parts;
        }

        private void AddParameters(SqlCommand command, PartMaster part)
        {
            command.Parameters.AddWithValue("@PartID", part.PartID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PartSerialNo", part.PartSerialNo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ProductID", part.ProductID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ModelID", part.ModelID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Prd_Color", part.Prd_Color ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PartCode", (object?)part.PartCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@PartDescription", (object?)part.PartDescription ?? DBNull.Value);
            command.Parameters.AddWithValue("@CustomerPrice", part.CustomerPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SPR_FactoryID", part.SPR_FactoryID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SPR_Tax", part.SPR_Tax ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SprColor", part.SprColor ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Spr_Cost", part.Spr_Cost ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Model", (object?)part.Model ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)part.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@Isuploaded", (object?)part.Isuploaded ?? DBNull.Value);
            command.Parameters.AddWithValue("@UploadedDateTime", part.UploadedDateTime ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CategoryID", part.CategoryID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Category", (object?)part.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@SFPrice", part.SFPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AVCost", part.AVCost ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PPrice", part.PPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MSLQty", part.MSLQty ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Location", (object?)part.Location ?? DBNull.Value);
            command.Parameters.AddWithValue("@Returnable", part.Returnable ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Tosell", part.Tosell ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MinLeadTime", part.MinLeadTime ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MaxLeadTime", part.MaxLeadTime ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AvgLeadTime", part.AvgLeadTime ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@MOQ", part.MOQ ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Repairable", part.Repairable ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@TallyCode", (object?)part.TallyCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@BOMQty", part.BOMQty ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsSerialized", part.IsSerialized ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsWallMount", part.IsWallMount ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@HSN", (object?)part.HSN ?? DBNull.Value);
            command.Parameters.AddWithValue("@Weight", part.Weight ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@BomType", (object?)part.BomType ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExWarrantyDays", part.ExWarrantyDays ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ValueApproval", part.ValueApproval ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SafteyBox", (object?)part.SafteyBox ?? DBNull.Value);
            command.Parameters.AddWithValue("@Paidwarranty", (object?)part.Paidwarranty ?? DBNull.Value);
        }

        private PartMaster MapToPartMaster(SqlDataReader reader)
        {
            return new PartMaster
            {
                PartID = reader["PartID"] as decimal?,
                PartSerialNo = reader["PartSerialNo"] as decimal?,
                ProductID = reader["ProductID"] as decimal?,
                ModelID = reader["ModelID"] as decimal?,
                Prd_Color = reader["Prd_Color"] as decimal?,
                PartCode = reader["PartCode"]?.ToString(),
                PartDescription = reader["PartDescription"]?.ToString(),
                CustomerPrice = reader["CustomerPrice"] as decimal?,
                SPR_FactoryID = reader["SPR_FactoryID"] as decimal?,
                SPR_Tax = reader["SPR_Tax"] as decimal?,
                SprColor = reader["SprColor"] as decimal?,
                Spr_Cost = reader["Spr_Cost"] as decimal?,
                Model = reader["Model"]?.ToString(),
                Status = reader["Status"]?.ToString(),
                Isuploaded = reader["Isuploaded"]?.ToString(),
                UploadedDateTime = reader["UploadedDateTime"] as DateTime?,
                CategoryID = reader["CategoryID"] as decimal?,
                Category = reader["Category"]?.ToString(),
                SFPrice = reader["SFPrice"] as decimal?,
                AVCost = reader["AVCost"] as decimal?,
                PPrice = reader["PPrice"] as decimal?,
                MSLQty = reader["MSLQty"] as decimal?,
                Location = reader["Location"]?.ToString(),
                Returnable = reader["Returnable"] as bool?,
                Tosell = reader["Tosell"] as bool?,
                MinLeadTime = reader["MinLeadTime"] as decimal?,
                MaxLeadTime = reader["MaxLeadTime"] as decimal?,
                AvgLeadTime = reader["AvgLeadTime"] as decimal?,
                MOQ = reader["MOQ"] as decimal?,
                Repairable = reader["Repairable"] as bool?,
                TallyCode = reader["TallyCode"]?.ToString(),
                BOMQty = reader["BOMQty"] as decimal?,
                IsSerialized = reader["IsSerialized"] as bool?,
                IsWallMount = reader["IsWallMount"] as bool?,
                HSN = reader["HSN"]?.ToString(),
                Weight = reader["Weight"] as decimal?,
                BomType = reader["BomType"]?.ToString(),
                ExWarrantyDays = reader["ExWarrantyDays"] as int?,
                ValueApproval = reader["ValueApproval"] as bool?,
                SafteyBox = reader["SafteyBox"]?.ToString(),
                Paidwarranty = reader["Paidwarranty"]?.ToString()
            };
        }
    }
}
