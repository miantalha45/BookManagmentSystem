using BookManagmentSystem.Data;
using HisaberAccountServer.Data;
using HisaberAccountServer.Models.LoginSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BookManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly BookDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, BookDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("Sign-Up")]
        public async Task<IActionResult> SignUp([FromBody] SignUp inParams)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inParams.Password) || inParams.Password.Length < 6)
                {
                    return Ok(new { status_code = 0, status_message = "Password must be at least 6 characters long." });
                }

                var userExists = await _userManager.FindByEmailAsync(inParams.Email);
                if (userExists != null)
                {
                    return Ok(new { status_code = 0, status_message = "Email already exists." });
                }

                var user = new ApplicationUser
                {
                    UserName = inParams.Email,
                    FullName = inParams.FullName,
                    Email = inParams.Email,
                    PhoneNumber = inParams.PhoneNumber,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(user, inParams.Password);

                if (!result.Succeeded)
                {
                    return Ok(new
                    {
                        status_code = 0,
                        status_message = "User registration failed",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Ok(new { status_message = "User Registered Successfully", status_code = 1, user });
            }
            catch (Exception e)
            {
                return Ok(new { status_message = "Sorry! Something went wrong..", status_code = 0, error = e.Message });
            }
        }

        [HttpPost("Sign-In")]
        public async Task<IActionResult> SignIn([FromBody] SignIn model)
        {
            try
            {

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Ok(new { status_message = "You have entered Invalid Email Address or Password", status_code = 0 });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
        {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new Claim("id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

                authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                        SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { Token = tokenString, status_code = 1, user, status_message = "Login Successful" });
            }
            catch (Exception e)
            {
                return Ok(new { status_message = e.Message, status_code = 0 });
            }
        }
    }
}