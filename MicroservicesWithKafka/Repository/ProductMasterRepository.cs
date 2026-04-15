using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ProductMasterRepository : IProductMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ProductMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task Add(ProductMasterModel model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ProductMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "insert");
            command.Parameters.AddWithValue("@Product", (object?)model.Product ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductCode", (object?)model.ProductCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_Category", (object?)model.Prd_Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_ModelID", (object?)model.Prd_ModelID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_colorID", (object?)model.Prd_colorID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_FactoryId", (object?)model.Prd_FactoryId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_CategoryID", (object?)model.Prd_CategoryID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_WtyDetail1", (object?)model.Prd_WtyDetail1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_WtyDetail2", (object?)model.Prd_WtyDetail2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_symptomID", (object?)model.Prd_symptomID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_causeid", (object?)model.Prd_causeid ?? DBNull.Value);
            command.Parameters.AddWithValue("@Prd_AnlysisId", (object?)model.Prd_AnlysisId ?? DBNull.Value);
            command.Parameters.AddWithValue("@Isuploaded", (object?)model.Isuploaded ?? DBNull.Value);
            command.Parameters.AddWithValue("@UploadedDateTime", (object?)model.UploadedDateTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@Status", (object?)model.Status ?? DBNull.Value);
            command.Parameters.AddWithValue("@AMC_Detail", (object?)model.AMC_Detail ?? DBNull.Value);
            command.Parameters.AddWithValue("@Extwty", (object?)model.Extwty ?? DBNull.Value);
            command.Parameters.AddWithValue("@BrandId", (object?)model.BrandId ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsEP", (object?)model.IsEP ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task AddMultiple(IEnumerable<ProductMasterModel> models)
        {
            foreach (var model in models)
            {
                await Add(model);
            }
        }

        public async Task<ProductMasterModel?> GetById(decimal id)
        {
            ProductMasterModel? product = null;

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ProductMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "getById");
            command.Parameters.AddWithValue("@ProductID", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                product = MapToProduct(reader);
            }

            return product;
        }

        public async Task<IEnumerable<ProductMasterModel>> GetAll()
        {
            var products = new List<ProductMasterModel>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ProductMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Type", "get");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task<IEnumerable<ProductMasterModel>> BrandCategoryId(decimal Brandid,decimal catid )
        {
            var products = new List<ProductMasterModel>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ProductMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Type", "getBrandCat");
            command.Parameters.AddWithValue("@Prd_CategoryID", catid);
            command.Parameters.AddWithValue("@BrandId", Brandid);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(MapToProduct(reader));
            }

            return products;
        }

        public async Task Delete(decimal id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_ProductMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "delete");
            command.Parameters.AddWithValue("@ProductID", id);

            await command.ExecuteNonQueryAsync();
        }

        private ProductMasterModel MapToProduct(SqlDataReader reader)
        {
            return new ProductMasterModel
            {
                ProductID = (decimal)reader["ProductID"],
                Product = reader["Product"]?.ToString(),
                ProductCode = reader["ProductCode"]?.ToString(),
                Prd_Category = reader["Prd_Category"]?.ToString(),
                Prd_ModelID = reader["Prd_ModelID"] as decimal?,
                Prd_colorID = reader["Prd_colorID"] as decimal?,
                Prd_FactoryId = reader["Prd_FactoryId"] as decimal?,
                Prd_CategoryID = reader["Prd_CategoryID"] as decimal?,
                Prd_WtyDetail1 = reader["Prd_WtyDetail1"]?.ToString(),
                Prd_WtyDetail2 = reader["Prd_WtyDetail2"]?.ToString(),
                Prd_symptomID = reader["Prd_symptomID"] as decimal?,
                Prd_causeid = reader["Prd_causeid"] as decimal?,
                Prd_AnlysisId = reader["Prd_AnlysisId"] as decimal?,
                Isuploaded = reader["Isuploaded"]?.ToString(),
                UploadedDateTime = reader["UploadedDateTime"] as DateTime?,
                Status = reader["Status"]?.ToString(),
                AMC_Detail = reader["AMC_Detail"]?.ToString(),
                Extwty = reader["Extwty"]?.ToString(),
                BrandId = reader["BrandId"] as decimal?,
                IsEP = reader["IsEP"] as bool?
            };
        }
    }
}
