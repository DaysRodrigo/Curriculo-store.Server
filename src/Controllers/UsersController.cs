using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Curriculo_store.Server.Models;
using Microsoft.AspNetCore.Authorization;

namespace Curriculo_store.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
    {
        private readonly UserManager<UserCrt> _userManager;

        public UsersController(UserManager<UserCrt> userManager)
        {
            _userManager = userManager;
        }

        //CREATE - POST : api/users
        [Authorize(Roles = "Master")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUser dto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new UserCrt
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Tipo = dto.Tipo
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                return Ok(new ResponseUser
                {
                   Name = $"Olá {dto.Name}"
                });
            }

            return BadRequest(result.Errors);
        }
    }
}