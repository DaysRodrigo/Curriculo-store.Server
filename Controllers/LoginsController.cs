using System;
using Curriculo_store.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Curriculo_store.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LoginController : ControllerBase
    {
        private readonly UserManager<UserCrt> _userManager;
        private readonly IConfiguration _configuration;

        public LoginController(UserManager<UserCrt> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        //LOGIN - POST: api/login
        [HttpPost]

        public async Task<IActionResult> Login([FromBody] LoginUser request)
        {
            if (request.Email == null || request.Password == null)
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            Console.WriteLine($"FindByEmailAsync result: {user}");

            if (user == null)
            {
                return Unauthorized("Usuário não encontrado.");
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return Unauthorized("Usuário ou senha incorretos.");
            }

            if ( result )
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Role, user.Tipo.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(3),
                    signingCredentials: creds
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new ResponseLogin
                {
                    Name = user.Name,
                    Email = user.Email!,
                    Tipo = user.Tipo,
                    Message = $"Sejá bem vindo {user.Name}",
                    Token = tokenString
                });
            }
            else
            {
                return Unauthorized("Usuário ou senha incorretos.");
            }

        }
    }
}