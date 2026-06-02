using Microsoft.AspNetCore.Mvc;
using MicroservicesWithKafka.Models;
using MicroservicesWithKafka.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MicroservicesWithKafka.Enums;


namespace MicroservicesWithKafka.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _config; // Added for JWT token settings

        public UsersController(UserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            return await _userService.GetAllUsersAsync();
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<User>> Get(string username)
        {
            var user = await _userService.GetUserByUserNameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("rsm")]
        public async Task<ActionResult<User>> GetRSM()
        {
            var user = await _userService.GetUserByRSMAsync();
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }


        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(Get), new { username = createdUser.UserName }, createdUser);
        }


        // password update
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("Username and NewPassword are required.");
            }

            var result = await _userService.UpdateUserAsync(new User
            {
                UserName = request.UserName,
                UPassword = request.NewPassword
            });

            return StatusCode(result.StatusCode, result);
        }

        // ✅ LOGIN API previous working  (Generates JWT Token)
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.UPassword))
        //        return BadRequest(new { message = "Username and password are required." });

        //    var response = await _userService.LoginUserAsync(request.UserName, request.UPassword);
        //    if (response == null || response.ReturnCode != 1)
        //        return Unauthorized(new { message = "Invalid credentials", response });

        //    // ✅ Add Role claim (based on database RoleName)
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name, response.UserName),
        //        new Claim("UserID", response.UserID.ToString()),
        //        new Claim(ClaimTypes.Role, response.RoleName ?? "CommonUser")
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _config["Jwt:Issuer"],
        //        audience: _config["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddHours(8),
        //        signingCredentials: creds
        //    );

        //    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        //    return Ok(new
        //    {
        //        response.ReturnCode,
        //        response.UserID,
        //        response.RoleID,
        //        response.UserName,
        //        response.RoleName,
        //        response.SF_ID,
        //        response.Branch_ID,
        //        response.ASMID,
        //        response.Cat_MapID,
        //        response.Message,
        //        token = tokenString
        //    });
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.UPassword))
                return BadRequest(new
                {
                    returnCode = 0,
                    userID = 0,
                    roleID = 0,
                    userName = request.UserName,
                    roleName = "",
                    sF_ID = 0,
                    branch_ID = 0,
                    asmid = 0,
                    cat_MapID = 0,
                    message = "Username and password are required.",
                    token = ""
                });

            var response = await _userService.LoginUserAsync(request.UserName, request.UPassword);
            if (response == null || response.ReturnCode != 1)
                return Unauthorized(new
                {
                    returnCode = 0,
                    userID = 0,
                    roleID = 0,
                    userName = request.UserName,
                    roleName = "",
                    sF_ID = 0,
                    branch_ID = 0,
                    asmid = 0,
                    cat_MapID = 0,
                    message = "Invalid credentials.",
                    token = ""
                });

            // 🔐 JWT Claims
            var claims = new List<Claim>
            {
                new Claim("username", response.UserName),
                new Claim("UserID", response.UserID.ToString()),
                new Claim("rolename", response.RoleName ?? "CommonUser"),
                new Claim("ASMID", response.ASMID.ToString()),
                new Claim("Cat_MapID", response.Cat_MapID.ToString()),
                new Claim("Branch_ID", response.Branch_ID.ToString()),
                new Claim("SF_ID", response.SF_ID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                returnCode = 1,
                userID = response.UserID,
                roleID = response.RoleID,
                userName = response.UserName,
                roleName = response.RoleName,
                sF_ID = response.SF_ID,
                branch_ID = response.Branch_ID,
                asmid = response.ASMID,
                cat_MapID = response.Cat_MapID,
                message = "Login successful.",
                token = tokenString
            });
        }



        // ✅ Protected API example
        [Authorize(Policy = "CommonUserPolicy")]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                message = $"Hello {username}, you are authorized with role: {role}"
            });
        }
    }
}
