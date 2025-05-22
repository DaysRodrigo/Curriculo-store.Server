using System;
using Curriculo_store.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StackExchange.Redis;
using Curriculo_store.Server.Services;

namespace Curriculo_store.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LoginController : ControllerBase
    {
        private readonly UserManager<UserCrt> _userManager;
        private readonly SignInManager<UserCrt> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RedisService _redis;

        public LoginController(UserManager<UserCrt> userManager, SignInManager<UserCrt> signInManager, IConfiguration configuration, RedisService redis)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _redis = redis;
        }

        //LOGIN - POST: api/login
        [HttpPost]

        public async Task<IActionResult> Login([FromBody] LoginUser request)
        {
            if (request.Email == null || request.Password == null)
            {
                return BadRequest("Email e senha s�o obrigat�rios.");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return Unauthorized("Usu�rio n�o encontrado.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Usu�rio ou senha incorretos.");
            }

            if (result.Succeeded)
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
                await _redis.SetTokenAsync(tokenString, "valid", TimeSpan.FromHours(3));

                return Ok(new ResponseLogin
                {
                    Name = user.Name,
                    Email = user.Email!,
                    Tipo = user.Tipo,
                    Message = $"Sej� bem vindo {user.Name}",
                    Token = tokenString
                });
            }
            else
            {
                return Unauthorized("Usu�rio ou senha incorretos.");
            }

        }
    }
}