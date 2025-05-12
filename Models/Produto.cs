using Curriculo_store.Server.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Curriculo_store.Server.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(25, MinimumLength = 2 , ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]

        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo é obrigatório.")]

        public TipoProduto Tipo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]

        public string? Descricao { get; set; }

        public string? FileUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}
