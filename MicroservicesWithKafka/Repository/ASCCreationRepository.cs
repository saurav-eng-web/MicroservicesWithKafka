using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Repository
{
    public class ASCCreationRepository : IASCCreationRepository
    {
        private readonly SQLConnectionService _sqlConnectionService;

        public ASCCreationRepository(SQLConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task<ASCCreation> CreateASCCreationAsync(ASCCreation asc)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASCCreation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "CREATE");

                    command.Parameters.AddWithValue("@ASC_Name", (object?)asc.ASC_Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_Owner_Name", (object?)asc.ASC_Owner_Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_Contact_No", (object?)asc.ASC_Contact_No ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_Address", (object?)asc.ASC_Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@State", (object?)asc.State ?? DBNull.Value);
                    command.Parameters.AddWithValue("@City", (object?)asc.City ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_PinCode", (object?)asc.ASC_PinCode ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_EMail", (object?)asc.ASC_EMail ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PanCardNo", (object?)asc.PanCardNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@GSTNo", (object?)asc.GSTNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AadharCardNo", (object?)asc.AadharCardNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LED", (object?)asc.LED ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AirConditioner", (object?)asc.AirConditioner ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Refrigerator", (object?)asc.Refrigerator ?? DBNull.Value);
                    command.Parameters.AddWithValue("@WashingMachine", (object?)asc.WashingMachine ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Speaker", (object?)asc.Speaker ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Cooler", (object?)asc.Cooler ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedBy", (object?)asc.CreatedBy ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MidifiedBy", (object?)asc.MidifiedBy ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", (object?)asc.Status ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            asc.RequestId = reader["RequestId"].ToString();
                            asc.Id = Convert.ToInt32(reader["Id"]);
                        }
                    }
                }
            }
            return asc;
        }


        public async Task<bool> UpdateASCCreationAsync(ASCCreation asc)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASCCreation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "UPDATE");
                    command.Parameters.AddWithValue("@Id", asc.Id);

                    command.Parameters.AddWithValue("@ASC_Name", (object?)asc.ASC_Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_Owner_Name", (object?)asc.ASC_Owner_Name ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_Contact_No", (object?)asc.ASC_Contact_No ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_Address", (object?)asc.ASC_Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@State", (object?)asc.State ?? DBNull.Value);
                    command.Parameters.AddWithValue("@City", (object?)asc.City ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_PinCode", (object?)asc.ASC_PinCode ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ASC_EMail", (object?)asc.ASC_EMail ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PanCardNo", (object?)asc.PanCardNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@GSTNo", (object?)asc.GSTNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AadharCardNo", (object?)asc.AadharCardNo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LED", (object?)asc.LED ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AirConditioner", (object?)asc.AirConditioner ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Refrigerator", (object?)asc.Refrigerator ?? DBNull.Value);
                    command.Parameters.AddWithValue("@WashingMachine", (object?)asc.WashingMachine ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Speaker", (object?)asc.Speaker ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Cooler", (object?)asc.Cooler ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MidifiedBy", (object?)asc.MidifiedBy ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Status", (object?)asc.Status ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Row exists → update succeeded
                            return true;
                        }
                    }
                }
            }

            return false;
        
        }

        public async Task<bool> DeleteASCCreationAsync(int id)
        {
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASCCreation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "DELETE");
                    command.Parameters.AddWithValue("@Id", id);

                    int rows = await command.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public async Task<ASCCreation?> GetASCCreationByIdAsync(int id)
        {
            ASCCreation? asc = null;
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASCCreation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "READ");
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            asc = new ASCCreation
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ASC_Name = reader["ASC_Name"].ToString(),
                                ASC_Owner_Name = reader["ASC_Owner_Name"].ToString(),
                                ASC_Contact_No = reader["ASC_Contact_No"].ToString(),
                                ASC_Address = reader["ASC_Address"].ToString(),
                                State = reader["State"].ToString(),
                                City = reader["City"].ToString(),
                                ASC_PinCode = reader["ASC_PinCode"].ToString(),
                                ASC_EMail = reader["ASC_EMail"].ToString(),
                                PanCardNo = reader["PanCardNo"].ToString(),
                                GSTNo = reader["GSTNo"].ToString(),
                                AadharCardNo = reader["AadharCardNo"].ToString(),
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                }
            }
            return asc;
        }

        public async Task<List<ASCCreation>> GetAllASCCreationsAsync()
        {
            var list = new List<ASCCreation>();
            using (var connection = _sqlConnectionService.GetConnection())
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("SP_CRUD_ASCCreation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Type", "get");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new ASCCreation
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ASC_Name = reader["ASC_Name"].ToString(),
                                ASC_Owner_Name = reader["ASC_Owner_Name"].ToString(),
                                ASC_Contact_No = reader["ASC_Contact_No"].ToString(),
                                State = reader["State"].ToString(),
                                City = reader["City"].ToString(),
                                ASC_EMail = reader["ASC_EMail"].ToString(),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
