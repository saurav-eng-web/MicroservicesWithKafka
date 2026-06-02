using System.Data;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using MicroservicesWithKafka.Services;
using Microsoft.Data.SqlClient;

public class UserRepository : IUserRepository
{
    private readonly SQLConnectionService _sqlConnectionService;

    public UserRepository(SQLConnectionService sqlConnectionService)
    {
        _sqlConnectionService = sqlConnectionService;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        using (var connection = _sqlConnectionService.GetConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("ManageUser", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Type", "insert");
                command.Parameters.AddWithValue("@Name", user.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Address", user.Address ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Mobno", user.Mobno ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Mail_ID", user.Mail_ID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserName", user.UserName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UPassword", user.UPassword ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@RoleID", user.RoleID);
                command.Parameters.AddWithValue("@Branch_ID", user.Branch_ID);
                command.Parameters.AddWithValue("@Branch_can_RegisterCall", user.Branch_can_RegisterCall);

                await command.ExecuteNonQueryAsync();
            }
        }

        return user;
    }

    public async Task<UpdateUserResponse> UpdateUserAsync(User user)
    {
        using (var connection = _sqlConnectionService.GetConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("ManageUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Type", "update");
                command.Parameters.AddWithValue("@Name", (object?)user.Name ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", (object?)user.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@Mobno", (object?)user.Mobno ?? DBNull.Value);
                command.Parameters.AddWithValue("@Mail_ID", (object?)user.Mail_ID ?? DBNull.Value);
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@UPassword", (object?)user.UPassword ?? DBNull.Value);
                command.Parameters.AddWithValue("@RoleID", (object?)user.RoleID ?? DBNull.Value);
                command.Parameters.AddWithValue("@Branch_ID", (object?)user.Branch_ID ?? DBNull.Value);
                command.Parameters.AddWithValue("@Branch_can_RegisterCall", (object?)user.Branch_can_RegisterCall ?? DBNull.Value);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new UpdateUserResponse
                        {
                            StatusCode = reader.GetInt32(reader.GetOrdinal("StatusCode")),
                            Message = reader.GetString(reader.GetOrdinal("Message")),
                            UserName = reader.IsDBNull(reader.GetOrdinal("UserName"))? null: reader.GetString(reader.GetOrdinal("UserName")),
                            Password = reader.IsDBNull(reader.GetOrdinal("UPassword"))? null: reader.GetString(reader.GetOrdinal("UPassword"))
                        };
                    }
                }
            }
        }

        return new UpdateUserResponse
        {
            StatusCode = 500,
            Message = "Unexpected error"
        };
    }
    public async Task<User> GetUserByUserNameAsync(string username)
    {
        User user = null;

        using (var connection = _sqlConnectionService.GetConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("ManageUser", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Type", "getuser");
                command.Parameters.AddWithValue("@UserName", username);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            Name = reader["Name"].ToString(),
                            Address = reader["Address"].ToString(),
                            Mobno = reader["Mobno"].ToString(),
                            Mail_ID = reader["Mail_ID"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            UPassword = reader["UPassword"].ToString(),
                            RoleID = Convert.ToInt32(reader["RoleID"]),
                            Branch_ID = Convert.ToInt32(reader["Branch_ID"]),
                            //Branch_can_RegisterCall = Convert.ToBoolean(reader["Branch_can_RegisterCall"]),
                           

                        };
                    }
                }
            }
        }

        return user;
    }



    public async Task<User> GetUserByRSMAsync()
    {
        User user = null;

        using (var connection = _sqlConnectionService.GetConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("ManageUser", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Type", "getuserByRSM");
                //command.Parameters.AddWithValue("@UserName", username);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            //Name = reader["Name"].ToString(),
                            //Address = reader["Address"].ToString(),
                            //Mobno = reader["Mobno"].ToString(),
                            //Mail_ID = reader["Mail_ID"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            UserId = reader["UserID"].ToString(),
                           // RoleID = Convert.ToInt32(reader["RoleID"]),
                           // Branch_ID = Convert.ToInt32(reader["Branch_ID"]),
                           // Branch_can_RegisterCall = Convert.ToBoolean(reader["Branch_can_RegisterCall"])
                        };
                    }
                }
            }
        }

        return user;
    }



    public async Task<UserLoginResponse> LoginUserAsync(string username, string password)
    {
        var result = new UserLoginResponse();

        using (var connection = _sqlConnectionService.GetConnection())
        {
            await connection.OpenAsync();

            using (var command = new SqlCommand("ManageUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Type", "Login");
                command.Parameters.AddWithValue("@UserName", username);
                command.Parameters.AddWithValue("@UPassword", password);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        result.ReturnCode = Convert.ToInt32(reader["ReturnCode"]);

                        if (result.ReturnCode == 1)
                        {
                            result.UserID = Convert.ToInt32(reader["UserID"]);
                            result.RoleID = Convert.ToInt32(reader["RoleID"]);
                            result.UserName = reader["UserName"].ToString();
                            result.RoleName = reader["RM_Name"].ToString();

                            result.SF_ID = reader["SF_ID"] != DBNull.Value ? Convert.ToInt32(reader["SF_ID"]) : 0;
                            result.Cat_MapID = reader["Cat_MapID"] != DBNull.Value ? Convert.ToInt32(reader["Cat_MapID"]) : 0;
                            result.ASMID = reader["ASMID"] != DBNull.Value ? Convert.ToInt32(reader["ASMID"]) : 0;
                            result.Branch_ID = reader["Branch_ID"] != DBNull.Value ? Convert.ToInt32(reader["Branch_ID"]) : 0;

                            result.Message = "Login successful.";
                        }
                        else
                        {
                            result.Message = "Invalid username or password.";
                        }
                    }
                    else
                    {
                        result.ReturnCode = -1;
                        result.Message = "No response from database.";
                    }
                }
            }
        }

        return result;
    }



    // Optional: Remove this if no use case exists, or update to call getuser for all usernames
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        throw new NotImplementedException("GetAllUsersAsync is not supported by the stored procedure.");
    }
}
