using MicroservicesWithKafka.Models;

public interface IUserRepository
{
    Task<User> GetUserByUserNameAsync(string username);
    Task<User> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);

    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<UserLoginResponse> LoginUserAsync(string username, string password);

    Task<User> GetUserByRSMAsync();


}
