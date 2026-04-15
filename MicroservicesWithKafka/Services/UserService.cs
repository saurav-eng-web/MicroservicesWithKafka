using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<User> GetUserByUserNameAsync(string username)
        {
            return await _userRepository.GetUserByUserNameAsync(username);
        }

        public async Task<User> GetUserByRSMAsync()
        {
            return await _userRepository.GetUserByRSMAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<UserLoginResponse> LoginUserAsync(string username, string password)
        {
            return await _userRepository.LoginUserAsync(username, password);
        }




    }
}
