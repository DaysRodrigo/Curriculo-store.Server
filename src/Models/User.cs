using System;
using Curriculo_store.Server.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Curriculo_store.Server.Models
{
    public class UserCrt : IdentityUser
    {

        [Required]
        [StringLength(100, MinimumLength = 11)]
        public string Name { get; set; } = string.Empty;


        [Required]
        public TipoUser Tipo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }

        public DateTime DeletedAt { get; set; }
    }
}