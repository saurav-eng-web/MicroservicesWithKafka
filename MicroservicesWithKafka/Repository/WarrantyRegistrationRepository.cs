using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class WarrantyRegistrationRepository : IWarrantyRegistrationRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public WarrantyRegistrationRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<long> InsertAsync(WarrantyRegistration model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_WarrantyRegistration", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "INSERT");
            command.Parameters.AddWithValue("@CustomerName", model.CustomerName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Pincode", model.Pincode ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@City", model.City ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CityID", model.CityID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Locality", model.Locality ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LocalityID", model.LocalityID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SerialNumber", model.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Product", model.Product ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ProductID", model.ProductID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Model", model.Model ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ModelID", model.ModelID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Tdate", model.Tdate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnterBy", model.EnterBy ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DOP", model.DOP ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PurchaseFromRetailer", model.PurchaseFromRetailer ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Invoice", model.Invoice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@BrandID", model.BrandID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Brand", model.Brand ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LastName", model.LastName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LandMark", model.LandMark ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Warranty", model.Warranty ?? (object)DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(WarrantyRegistration model)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_WarrantyRegistration", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "UPDATE");
            command.Parameters.AddWithValue("@ID", model.ID);
            command.Parameters.AddWithValue("@CustomerName", model.CustomerName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Address", model.Address ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Pincode", model.Pincode ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@City", model.City ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CityID", model.CityID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Locality", model.Locality ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LocalityID", model.LocalityID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@SerialNumber", model.SerialNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Product", model.Product ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ProductID", model.ProductID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Model", model.Model ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ModelID", model.ModelID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Tdate", model.Tdate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EnterBy", model.EnterBy ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DOP", model.DOP ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PurchaseFromRetailer", model.PurchaseFromRetailer ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Invoice", model.Invoice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@BrandID", model.BrandID ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Brand", model.Brand ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LastName", model.LastName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LandMark", model.LandMark ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Warranty", model.Warranty ?? (object)DBNull.Value);

            // 🛠 Make sure to return rows affected
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var rows = Convert.ToInt32(reader["RowsAffected"]);
                return rows > 0;
            }

            return false;
        }


        public async Task<bool> DeleteAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_WarrantyRegistration", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "DELETE");
            command.Parameters.AddWithValue("@ID", id);

            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Convert.ToInt32(reader["RowsAffected"]) > 0;
            }
            return false;
        }

        public async Task<List<WarrantyRegistration>> GetAllAsync()
        {
            var list = new List<WarrantyRegistration>();
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_WarrantyRegistration", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Type", "GET_ALL");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToModel(reader));
            }

            return list;
        }

        public async Task<WarrantyRegistration?> GetByIdAsync(long id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_WarrantyRegistration", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Type", "GET_BY_ID");
            command.Parameters.AddWithValue("@ID", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToModel(reader);
            }

            return null;
        }

        public async Task<WarrantyRegistration?> GetBySerialNumberAsync(string serialNumber)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_WarrantyRegistration", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Type", "GET_BY_SERIAL");
            command.Parameters.AddWithValue("@SerialNumber", serialNumber);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToModel(reader);
            }

            return null;
        }

        private WarrantyRegistration MapToModel(SqlDataReader reader)
        {
            return new WarrantyRegistration
            {
                ID = Convert.ToInt64(reader["ID"]),
                CustomerName = reader["CustomerName"]?.ToString(),
                PhoneNumber = reader["PhoneNumber"] as long?,
                Address = reader["Address"]?.ToString(),
                Pincode = reader["Pincode"] as long?,
                City = reader["City"]?.ToString(),
                CityID = reader["CityID"] as long?,
                Locality = reader["Locality"]?.ToString(),
                LocalityID = reader["LocalityID"] as long?,
                SerialNumber = reader["SerialNumber"]?.ToString(),
                Product = reader["Product"]?.ToString(),
                ProductID = reader["ProductID"] as long?,
                Model = reader["Model"]?.ToString(),
                ModelID = reader["ModelID"] as long?,
                Tdate = reader["Tdate"] as DateTime?,
                EnterBy = reader["EnterBy"]?.ToString(),
                DOP = reader["DOP"] as DateTime?,
                PurchaseFromRetailer = reader["PurchaseFromRetailer"]?.ToString(),
                Invoice = reader["Invoice"]?.ToString(),
                BrandID = reader["BrandID"] as long?,
                Brand = reader["Brand"]?.ToString(),
                LastName = reader["LastName"]?.ToString(),
                LandMark = reader["LandMark"]?.ToString(),
                Warranty = reader["Warranty"]?.ToString()
            };
        }
    }
}
