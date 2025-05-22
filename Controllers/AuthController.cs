using System;
using Curriculo_store.Server.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]

public class AuthController: ControllerBase
{
    private readonly RedisService _redis;

    public AuthController(RedisService redis)
    {
        _redis = redis;
    }

    [HttpGet("validate/token")]
    public async Task<IActionResult> TokenValidation([FromHeader(Name = "Authorization")] string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token Ausent");
        }

        if (token.StartsWith("Bearer"))
        {
            token = token.Substring("Bearer ".Length);
        }

        var exist = await _redis.TokenExistAsync(token);

        if (!exist)
        {
            return Unauthorized("Invalid or expired token");
        }

        return Ok("Token validated");
    }

    [HttpPost("register/token")]

    public async Task<IActionResult> SaveToken([FromHeader (Name = "Authorization")] string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token ausente");
        }

        if (token.StartsWith("Bearer"))
        {
            token = token.Substring("Bearer ".Length);
        }

        await _redis.SetTokenAsync(token, "valid", TimeSpan.FromMinutes(10));
        return Ok("Token registrado");
    }

}