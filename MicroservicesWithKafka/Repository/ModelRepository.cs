using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ModelRepository : IModelRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ModelRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private SqlCommand BuildCommand(SqlConnection connection, string action, ModelMasterViewModel model)
        {
            var command = new SqlCommand("usp_ModelMaster_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", action);
            command.Parameters.AddWithValue("@ModelID", (object?)model.ModelID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductID", (object?)model.ProductID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModelNo", (object?)model.ModelNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@ManfBy", (object?)model.ManfBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@PM", (object?)model.PM ?? DBNull.Value);
            command.Parameters.AddWithValue("@CW", (object?)model.CW ?? DBNull.Value);
            command.Parameters.AddWithValue("@NCW", (object?)model.NCW ?? DBNull.Value);
            command.Parameters.AddWithValue("@PMMonth", (object?)model.PMMonth ?? DBNull.Value);
            command.Parameters.AddWithValue("@CWMonth", (object?)model.CWMonth ?? DBNull.Value);
            command.Parameters.AddWithValue("@NCWMonth", (object?)model.NCWMonth ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsUploaded", (object?)model.IsUploaded ?? DBNull.Value);
            command.Parameters.AddWithValue("@UploadedDateTime", (object?)model.UploadedDateTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@ColorId", (object?)model.ColorId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CategoryId", (object?)model.CategoryId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModelCode", (object?)model.ModelCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@SerialNo", (object?)model.SerialNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@FactoryId", (object?)model.FactoryId ?? DBNull.Value);
            command.Parameters.AddWithValue("@SerialLimit", (object?)model.SerialLimit ?? DBNull.Value);
            command.Parameters.AddWithValue("@SerialMaxLimit", (object?)model.SerialMaxLimit ?? DBNull.Value);
            command.Parameters.AddWithValue("@Hi_End", (object?)model.Hi_End ?? DBNull.Value);
            command.Parameters.AddWithValue("@Low_end", (object?)model.Low_end ?? DBNull.Value);
            command.Parameters.AddWithValue("@Rate_LT250LFF", (object?)model.Rate_LT250LFF ?? DBNull.Value);
            command.Parameters.AddWithValue("@Rate_GT250_LT450LFF", (object?)model.Rate_GT250_LT450LFF ?? DBNull.Value);
            command.Parameters.AddWithValue("@Rate_GT450LFF", (object?)model.Rate_GT450LFF ?? DBNull.Value);
            command.Parameters.AddWithValue("@Ctv14To21", (object?)model.Ctv14To21 ?? DBNull.Value);
            command.Parameters.AddWithValue("@Ctv25To34", (object?)model.Ctv25To34 ?? DBNull.Value);
            command.Parameters.AddWithValue("@ServCharForRoylty", (object?)model.ServCharForRoylty ?? DBNull.Value);
            command.Parameters.AddWithValue("@SubProductID", (object?)model.SubProductID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModelLaunchdate", (object?)model.ModelLaunchdate ?? DBNull.Value);
            command.Parameters.AddWithValue("@EndOfDate", (object?)model.EndOfDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", (object?)model.Price ?? DBNull.Value);
            command.Parameters.AddWithValue("@HSN", (object?)model.HSN ?? DBNull.Value);
            command.Parameters.AddWithValue("@TaxPer", (object?)model.TaxPer ?? DBNull.Value);
            command.Parameters.AddWithValue("@Weight", (object?)model.Weight ?? DBNull.Value);

            return command;
        }

        public async Task<ModelMasterViewModel> CreateModelAsync(ModelMasterViewModel model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "INSERT", model);
            await command.ExecuteNonQueryAsync();

            return model;
        }

        public async Task<bool> UpdateModelAsync(ModelMasterViewModel model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "UPDATE", model);

            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) == 1;
        }


        public async Task<bool> DeleteModelAsync(decimal modelId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelMaster_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "DELETE");
            command.Parameters.AddWithValue("@ModelID", modelId);

            try
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<ModelMasterViewModel> GetModelByIdAsync(decimal modelId)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelMaster_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "SELECT");
            command.Parameters.AddWithValue("@ModelID", modelId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToModel(reader);
            }

            return null;
        }

        public async Task<List<ModelMasterViewModel>> GetModelsByProductIdAsync(decimal productId)
        {
            var models = new List<ModelMasterViewModel>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelMaster_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "SELECTP");
            command.Parameters.AddWithValue("@ProductID", productId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                models.Add(MapReaderToModel(reader));
            }

            return models;
        }


       

        public async Task<List<ModelMasterViewModel>> GetAllModelsAsync()
        {
            var models = new List<ModelMasterViewModel>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("usp_ModelMaster_CRUD", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Action", "SELECT");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                models.Add(MapReaderToModel(reader));
            }

            return models;
        }

        private ModelMasterViewModel MapReaderToModel(SqlDataReader reader)
        {
            return new ModelMasterViewModel
            {
                ModelID = reader["ModelID"] as decimal?,
                ProductID = reader["ProductID"] as decimal?,
                ModelNo = reader["ModelNo"]?.ToString(),
                Description = reader["description"]?.ToString(),
                ManfBy = reader["manfby"]?.ToString(),
                PM = reader["PM"]?.ToString(),
                CW = reader["CW"]?.ToString(),
                NCW = reader["NCW"]?.ToString(),
                PMMonth = reader["PMMonth"]?.ToString(),
                CWMonth = reader["CWMonth"]?.ToString(),
                NCWMonth = reader["NCWMonth"]?.ToString(),
                Status = reader["Status"]?.ToString(),
                IsUploaded = reader["Isuploaded"]?.ToString(),
                UploadedDateTime = Convert.ToDateTime(reader["UploadedDateTime"]),
                ColorId = reader["ColorId"] as decimal?,
                CategoryId = reader["CategoryId"] as decimal?,
                ModelCode = reader["ModelCode"]?.ToString(),
                SerialNo = reader["SerialNo"] as decimal?,
                FactoryId = reader["FactoryId"] as decimal?,
                SerialLimit = Convert.ToDecimal(reader["SerialLimit"]),
                SerialMaxLimit = reader["SerialMaxLimit"] as decimal?,
                Hi_End = reader["Hi_End"] as bool?,
                Low_end = reader["Low_end"] as bool?,
                Rate_LT250LFF = reader["Rate_LT250LFF"] as bool?,
                Rate_GT250_LT450LFF = reader["Rate_GT250_LT450LFF"] as bool?,
                Rate_GT450LFF = reader["Rate_GT450LFF"] as bool?,
                Ctv14To21 = reader["Ctv14To21"] as bool?,
                Ctv25To34 = reader["Ctv25To34"] as bool?,
                ServCharForRoylty = reader["ServCharForRoylty"] as decimal?,
                SubProductID = reader["SubProductID"] as decimal?,
                ModelLaunchdate = reader["ModelLaunchdate"] as DateTime?,
                EndOfDate = reader["EndOfDate"] as DateTime?,
                Price = reader["Price"] as decimal?,
                HSN = reader["HSN"]?.ToString(),
                TaxPer = reader["TaxPer"] as decimal?,
                Weight = reader["Weight"] as decimal?
            };
        }
    }
}
