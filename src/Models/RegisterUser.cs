using System;
using System.ComponentModel.DataAnnotations;
using Curriculo_store.Server.Shared.Enums;
using Microsoft.AspNetCore.Identity;

public class RegisterUser
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 11)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public TipoUser Tipo { get; set; }
}

