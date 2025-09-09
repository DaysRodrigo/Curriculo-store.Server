using Curriculo_store.Server.Shared.Enums;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Curriculo_store.Server.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]

        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo é obrigatório.")]

        public TipoProduto Tipo { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]

        public string Descricao { get; set; } = string.Empty;

        public string? FileUrl { get; set; }

        [Required(ErrorMessage = "É necessário o nome da instituição.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "O nome da instituição deve ter no máximo 50 caracteres e no mínimo 5")]
        public string Instituicao { get; set; } = string.Empty;

        [Required(ErrorMessage = "É necessário um valor. Correto ou simbólico.")]
        public float Valor { get; set; }

        [Required(ErrorMessage = "É necessário definir um período.")]

        public string Periodo { get; set; } = string.Empty;

        [Required(ErrorMessage = "É necessário definir as tecnologias.")]

        public string Tecnologias { get; set; } = string.Empty;

        [NotMapped]
        public List<string> TecnologiasList
        {
            get => string.IsNullOrEmpty(Tecnologias)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(Tecnologias)!;
            set => Tecnologias = JsonSerializer.Serialize(value);
        }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}
