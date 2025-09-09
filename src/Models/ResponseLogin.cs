using System;
using System.ComponentModel.DataAnnotations;
using Curriculo_store.Server.Shared.Enums;
using Microsoft.AspNetCore.Identity;
//DTO for return on login successfully
public class ResponseLogin
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public TipoUser Tipo { get; set; }
    public string Token { get; set; }
}

