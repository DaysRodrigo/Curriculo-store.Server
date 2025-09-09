using System;
using System.ComponentModel.DataAnnotations;

namespace Curriculo_store.Server.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}