using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class BrandMasterRepository : IBrandMasterRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public BrandMasterRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<BrandMasterViewModel> CreateBrandAsync(BrandMasterViewModel brand)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_BandMaster", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Type", "insert");
                    command.Parameters.AddWithValue("@Brand_Name", (object?)brand.Brand_Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Brand_Contact", (object?)brand.Brand_Contact ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RegCC", brand.RegCC ?? false);
                    command.Parameters.AddWithValue("@RegSF", brand.RegSF ?? false);
                    command.Parameters.AddWithValue("@Active", brand.Active ?? true);
                    command.Parameters.AddWithValue("@ComplaintStart", (object?)brand.ComplaintStart ?? DBNull.Value);

                    var result = await command.ExecuteScalarAsync();
                    brand.Brand_ID = Convert.ToDecimal(result);
                }
            }

            return brand;
        }
        public async Task<bool> UpdateBrandAsync(BrandMasterViewModel brand)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_BandMaster", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Type", "update");
                    command.Parameters.AddWithValue("@Brand_ID", brand.Brand_ID ?? 0);
                    command.Parameters.AddWithValue("@Brand_Name", (object?)brand.Brand_Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Brand_Contact", (object?)brand.Brand_Contact ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RegCC", brand.RegCC ?? false);
                    command.Parameters.AddWithValue("@RegSF", brand.RegSF ?? false);
                    command.Parameters.AddWithValue("@Active", brand.Active ?? true);
                    command.Parameters.AddWithValue("@ComplaintStart", (object?)brand.ComplaintStart ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                            return rows > 0;
                        }
                    }

                    return false;
                }
            }
        }

        //public async Task<bool> DeleteBrandAsync(decimal brandId)
        //{
        //    using (var connection = _sqlConnectionService.GetConnection())
        //    {
        //        await connection.OpenAsync();

        //        using (var command = new SqlCommand("SP_CRUD_BandMaster", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;

        //            command.Parameters.AddWithValue("@Type", "delete");
        //            command.Parameters.AddWithValue("@Brand_ID", brandId);

        //            int rowsAffected = await command.ExecuteNonQueryAsync();
        //            return rowsAffected > 0;
        //        }
        //    }
        //}

        public async Task<bool> DeleteBrandAsync(decimal brandId)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_BandMaster", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Type", "delete");
                    command.Parameters.AddWithValue("@Brand_ID", brandId);

                    // Add dummy values for optional parameters (to avoid NULL issues)
                    command.Parameters.AddWithValue("@Brand_Name", DBNull.Value);
                    command.Parameters.AddWithValue("@Brand_Contact", DBNull.Value);
                    command.Parameters.AddWithValue("@RegCC", DBNull.Value);
                    command.Parameters.AddWithValue("@RegSF", DBNull.Value);
                    command.Parameters.AddWithValue("@Active", DBNull.Value);
                    command.Parameters.AddWithValue("@ComplaintStart", DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                            return rows > 0;
                        }
                    }

                    return false;
                }
            }
        }



        public async Task<BrandMasterViewModel?> GetBrandByIdAsync(decimal brandId)
        {
            BrandMasterViewModel? brand = null;

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_BandMaster", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "getbyid");
                    command.Parameters.AddWithValue("@Brand_ID", brandId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            brand = new BrandMasterViewModel
                            {
                                Brand_ID = reader["Brand_ID"] as decimal?,
                                Brand_Name = reader["Brand_Name"]?.ToString(),
                                Brand_Contact = reader["Brand_Contact"]?.ToString(),
                                RegCC = reader["RegCC"] as bool?,
                                RegSF = reader["RegSF"] as bool?,
                                Active = reader["Active"] as bool?,
                                ComplaintStart = reader["ComplaintStart"]?.ToString()
                            };
                        }
                    }
                }
            }

            return brand;
        }

        public async Task<List<BrandMasterViewModel>> GetAllBrandsAsync()
        {
            var brands = new List<BrandMasterViewModel>();

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_BandMaster", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "get");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            brands.Add(new BrandMasterViewModel
                            {
                                Brand_ID = reader["Brand_ID"] as decimal?,
                                Brand_Name = reader["Brand_Name"]?.ToString(),
                                Brand_Contact = reader["Brand_Contact"]?.ToString(),
                                RegCC = reader["RegCC"] as bool?,
                                RegSF = reader["RegSF"] as bool?,
                                Active = reader["Active"] as bool?,
                                ComplaintStart = reader["ComplaintStart"]?.ToString()
                            });
                        }
                    }
                }
            }

            return brands;
        }
    }
}
