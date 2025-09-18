using Curriculo_store.Server.Shared.Enums;

namespace Curriculo_store.Server.Models.Dtos
{
    public class ProdutoUpdateDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public TipoProduto Tipo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public string Instituicao { get; set; } = string.Empty;
        public float Valor { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public List<string> Tecnologias { get; set; } = new List<string>();
    }
}