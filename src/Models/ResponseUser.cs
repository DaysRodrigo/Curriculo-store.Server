using System;
using System.ComponentModel.DataAnnotations;
using Curriculo_store.Server.Shared.Enums;
using Microsoft.AspNetCore.Identity;
//DTO for return on user creation
public class ResponseUser
{
    public string Name { get; set; }
}

