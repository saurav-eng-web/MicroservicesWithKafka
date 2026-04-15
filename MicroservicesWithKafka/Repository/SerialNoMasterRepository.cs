using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class SerialNoMasterRepository : ISerialNoMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public SerialNoMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        private SqlCommand BuildCommand(SqlConnection connection, string action, SerialNoMaster serial)
        {
            var command = new SqlCommand("SP_CRUD_SerialNoMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", action);
            command.Parameters.AddWithValue("@Id", serial.Id);
            command.Parameters.AddWithValue("@Brand", (object?)serial.Brand ?? DBNull.Value);
            command.Parameters.AddWithValue("@BrandID", (object?)serial.BrandID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Model", (object?)serial.Model ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModelID", (object?)serial.ModelID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductCategory", (object?)serial.ProductCategory ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductCategoryID", (object?)serial.ProductCategoryID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Product", (object?)serial.Product ?? DBNull.Value);
            command.Parameters.AddWithValue("@ProductID", (object?)serial.ProductID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ItemCode", (object?)serial.ItemCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ItemSerialNo1", (object?)serial.ItemSerialNo1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@ItemSerialNo2", (object?)serial.ItemSerialNo2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@IMEI1", (object?)serial.IMEI1 ?? DBNull.Value);
            command.Parameters.AddWithValue("@IMEI2", (object?)serial.IMEI2 ?? DBNull.Value);
            command.Parameters.AddWithValue("@BatchNo", (object?)serial.BatchNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@BoxID", (object?)serial.BoxID ?? DBNull.Value);
            command.Parameters.AddWithValue("@ParentItemCode", (object?)serial.ParentItemCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@ParentSerialNo", (object?)serial.ParentSerialNo ?? DBNull.Value);
            command.Parameters.AddWithValue("@TypeData", (object?)serial.Type ?? DBNull.Value);
            command.Parameters.AddWithValue("@Tdate", (object?)serial.Tdate ?? DBNull.Value);
            command.Parameters.AddWithValue("@OldSerialNumber", (object?)serial.OldSerialNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@BomType", (object?)serial.BomType ?? DBNull.Value);
            command.Parameters.AddWithValue("@purchstatus", (object?)serial.purchstatus ?? DBNull.Value);

            return command;
        }

        public async Task<SerialNoMaster> CreateAsync(SerialNoMaster serial)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "insert", serial);
            var result = await command.ExecuteScalarAsync();
            serial.Id = Convert.ToDecimal(result);
            return serial;
        }

        public async Task<bool> UpdateAsync(SerialNoMaster serial)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = BuildCommand(connection, "update", serial);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(decimal id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_SerialNoMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "delete");
            command.Parameters.AddWithValue("@Id", id);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<SerialNoMaster?> GetByIdAsync(decimal id)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_SerialNoMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "getbyid");
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToSerial(reader);
            }

            return null;
        }

        public async Task<List<SerialNoMaster>> GetAllAsync()
        {
            var list = new List<SerialNoMaster>();

            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_SerialNoMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "get");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapToSerial(reader));
            }

            return list;
        }

        public async Task<SerialNoMaster?> GetBySerialNumberAsync(string serialNo)
        {
            using var connection = _sqlConnectionService.GetConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand("SP_CRUD_SerialNoMaster", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Type", "getbyserialno");
            command.Parameters.AddWithValue("@ItemSerialNo1", serialNo);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToSerial(reader);
            }

            return null;
        }


        private SerialNoMaster MapToSerial(SqlDataReader reader)
        {
            return new SerialNoMaster
            {
                Id = reader["Id"] as decimal?,
                Brand = reader["Brand"]?.ToString(),
                BrandID = reader["BrandID"] as decimal?,
                Model = reader["Model"]?.ToString(),
                ModelID = reader["ModelID"] as decimal?,
                ProductCategory = reader["ProductCategory"]?.ToString(),
                ProductCategoryID = reader["ProductCategoryID"] as decimal?,
                Product = reader["Product"]?.ToString(),
                ProductID = reader["ProductID"] as decimal?,
                ItemCode = reader["ItemCode"]?.ToString(),
                ItemSerialNo1 = reader["ItemSerialNo1"]?.ToString(),
                ItemSerialNo2 = reader["ItemSerialNo2"]?.ToString(),
                IMEI1 = reader["IMEI1"] as decimal?,
                IMEI2 = reader["IMEI2"] as decimal?,
                BatchNo = reader["BatchNo"]?.ToString(),
                BoxID = reader["BoxID"]?.ToString(),
                ParentItemCode = reader["ParentItemCode"]?.ToString(),
                ParentSerialNo = reader["ParentSerialNo"]?.ToString(),
                Type = reader["Type"]?.ToString(),
                Tdate = reader["Tdate"] as DateTime?,
                OldSerialNumber = reader["OldSerialNumber"]?.ToString(),
                BomType = reader["BomType"]?.ToString(),
                purchstatus = reader["purchstatus"]?.ToString(),
                Modeldes= reader["Modeldes"]?.ToString()

            };
        }
    }
}
