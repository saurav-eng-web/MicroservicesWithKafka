using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public CategoryRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<CategoryMaster> CreateCategoryAsync(CategoryMaster category)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ManageCategory", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Type", "insert");
                    command.Parameters.AddWithValue("@Cat_Name", category.Cat_Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cat_BrandID", category.Cat_BrandID);
                    command.Parameters.AddWithValue("@Cat_Code", category.Cat_Code ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@cat_Status", category.cat_Status);
                    command.Parameters.AddWithValue("@Cat_FullName", category.Cat_FullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ForB2B", category.ForB2B);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return category;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryMaster category)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ManageCategory", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Type", "update");
                    command.Parameters.AddWithValue("@Cat_ID", category.Cat_ID ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cat_Name", category.Cat_Name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cat_BrandID", category.Cat_BrandID);
                    command.Parameters.AddWithValue("@Cat_Code", category.Cat_Code ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@cat_Status", category.cat_Status);
                    command.Parameters.AddWithValue("@Cat_FullName", category.Cat_FullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ForB2B", category.ForB2B);

                    try
                    {
                        int rows = await command.ExecuteNonQueryAsync();
                        // Treat 0 rows as success too, just no change
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }


        public async Task<CategoryMaster> GetCategoryByIdAsync(decimal catId)
        {
            CategoryMaster category = null;

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ManageCategory", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Type", "getbyid");
                    command.Parameters.AddWithValue("@Cat_ID", catId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            category = new CategoryMaster
                            {
                                Cat_ID = reader["Cat_ID"] as decimal?,
                                Cat_Name = reader["Cat_Name"].ToString(),
                                Cat_BrandID = Convert.ToDecimal(reader["Cat_BrandID"]),
                                Cat_Code = reader["Cat_Code"].ToString(),
                                cat_Status = Convert.ToBoolean(reader["cat_Status"]),
                                Cat_FullName = reader["Cat_FullName"].ToString(),
                                ForB2B = Convert.ToBoolean(reader["ForB2B"])
                            };
                        }
                    }
                }
            }



            return category;
        }

        public async Task<List<CategoryMaster>> GetAllCategoriesAsync()
        {
            var categories = new List<CategoryMaster>();

            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("ManageCategory", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "getall");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categories.Add(new CategoryMaster
                            {
                                Cat_ID = reader["Cat_ID"] as decimal?,
                                Cat_Name = reader["Cat_Name"].ToString(),
                                Cat_BrandID = Convert.ToDecimal(reader["Cat_BrandID"]),
                                Cat_Code = reader["Cat_Code"].ToString(),
                                cat_Status = Convert.ToBoolean(reader["cat_Status"]),
                                Cat_FullName = reader["Cat_FullName"].ToString(),
                                ForB2B = Convert.ToBoolean(reader["ForB2B"])
                            });
                        }
                    }
                }
            }

            return categories;
        }

    }
}
