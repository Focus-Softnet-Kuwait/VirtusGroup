using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VirtusGroup.API.Models;
namespace VirtusGroup.API.Controllers
{
    public class LoginController : ApiController
    {
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public LoginController()
        {
            // Assuming you're using ConfigurationManager for .NET Framework
            _jwtKey = ConfigurationManager.AppSettings["Jwt:Key"];
            _jwtIssuer = ConfigurationManager.AppSettings["Jwt:Issuer"];
            _jwtAudience = ConfigurationManager.AppSettings["Jwt:Audience"];
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> Login([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Invalid request.");
            }

            // Check if model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Authenticate user (replace with actual authentication logic)
            var authenticatedUser = await AuthenticateUserAsync(user);
            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            // Generate JWT token (implement your JWT generation logic)
            var tokenString = GenerateJwtToken(authenticatedUser);
            return Ok(new { Token = tokenString });
        }

        private async Task<User> AuthenticateUserAsync(User user)
        {
            // Implement your user authentication logic here
            // For example, you can check the user credentials against a database
            // This is just a placeholder implementation
            if (user.Username == "admin" && user.Password == "12345678")
            {
                return new User { Username = "admin" };
            }
            return null;
        }

        private string GenerateJwtToken(User user)
        {
            // Implement your JWT token generation logic here
            // This is just a placeholder implementation
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
