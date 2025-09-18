using Curriculo_store.Server.Models;
using Curriculo_store.Server.Shared.Enums;
using Curriculo_store.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Curriculo_store.Server.Models.Dtos;

namespace Curriculo_store.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //CREATE - POST: api/produtos
        [HttpPost]

        public async Task<IActionResult> CreateProduto([FromBody] ProdutoUpdateDto produto)
        {
            var toCrt = new Produto();
            if (produto == null)
            {
                return BadRequest("Produto inválido.");
            }

            var existe = await _context.Produtos
                .AnyAsync(p => p.Nome.Trim().ToLower() == produto.Nome.Trim().ToLower()
                        && p.Tipo == produto.Tipo);

            if (existe)
            {
                return Conflict("Este produto já existe.");
            }

            if ((produto.Tipo == TipoProduto.Curso && string.IsNullOrWhiteSpace(produto.FileUrl)) ||
                (produto.Tipo == TipoProduto.Academico && string.IsNullOrWhiteSpace(produto.FileUrl)))
            {
                return BadRequest("Para este tipo, é necessário fornecer um arquivo.");
            }
            else
            {
                toCrt.Nome = produto.Nome;
                toCrt.Tipo = produto.Tipo;
                toCrt.Descricao = produto.Descricao;
                toCrt.Periodo = produto.Periodo;
                toCrt.FileUrl = produto.FileUrl;
                toCrt.Valor = produto.Valor;
                toCrt.Instituicao = produto.Instituicao;
            }

            if (produto.Tecnologias.IsNullOrEmpty())
            {
                return BadRequest("É necessário definir as tecnologias.");
            }
            else
            {
                var teste = string.Join(", ", produto.Tecnologias);

                toCrt.Tecnologias = teste;
            }

            _context.Produtos.Add(toCrt);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProdutoById), new { id = toCrt.Id }, toCrt);
        }

        //READ ONE - GET: api/produtos/{id}
        [HttpGet("{id}")]

        public async Task<ActionResult<Produto>> GetProdutoById(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        //READ ALL - GET: api/produtos/all
        [HttpGet("all")]

        public async Task<ActionResult<List<Produto>>> GetAllProdutos()
        {
            var produtos = await _context.Produtos
                .Where(p => p.DeletedAt == null)
                .AsNoTracking()
                .ToListAsync();

            if (produtos == null || produtos.Count == 0)
            {
                return NoContent();
            }

            return Ok(produtos);
        }

        //UPDATE BY ID - PUT: api/produtos/{id}

        [HttpPut("{id}")]

        public async Task<ActionResult<Produto>> UpdateProdutoById(int id, [FromBody] ProdutoUpdateDto updatedProduto)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            if (produto.DeletedAt != null)
            {
                return BadRequest("Não é possível atualizar um produto excluído.");
            }


            bool hasChanges = false;

            bool UpdateIfChanged(string original, string updated, Action<string> setter)
            {
                var originalTrimmed = original?.Trim() ?? string.Empty;
                var updatedTrimmed = updated?.Trim() ?? string.Empty;

                if (originalTrimmed != updatedTrimmed)
                {
                    setter(updated);
                    return true;
                }
                return false;
            }

            bool TecnologiasChanged(string originalString, List<string> updatedArray)
            {
                var originalList = originalString?
                    .Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList() ?? new List<string>();

                var sortedOriginal = originalList.OrderBy(t => t).ToList();
                var sortedUpdated = updatedArray.OrderBy(t => t).ToList();

                return !sortedOriginal.SequenceEqual(sortedUpdated);
            }


            hasChanges |= UpdateIfChanged(produto.Nome, updatedProduto.Nome, value => produto.Nome = value);
            hasChanges |= UpdateIfChanged(produto.Descricao, updatedProduto.Descricao, value => produto.Descricao = value);
            hasChanges |= UpdateIfChanged(produto.FileUrl, updatedProduto.FileUrl, value => produto.FileUrl = value);
            hasChanges |= UpdateIfChanged(produto.Instituicao, updatedProduto.Instituicao, value => produto.Instituicao = value);
            hasChanges |= UpdateIfChanged(produto.Periodo, updatedProduto.Periodo, value => produto.Periodo = value);
            hasChanges |= TecnologiasChanged(produto.Tecnologias, updatedProduto.Tecnologias);

            if (hasChanges)
            {
                produto.Tecnologias = string.Join(", ", updatedProduto.Tecnologias);
            }

            if (produto.Tipo != updatedProduto.Tipo)
            {
                produto.Tipo = updatedProduto.Tipo;
                hasChanges = true;
            }

            if (produto.Valor != updatedProduto.Valor)
            {
                produto.Valor = updatedProduto.Valor;
                hasChanges = true;
            }

            if (hasChanges)
            {
                produto.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return Ok(produto);
        }

        //MASS UPDATE - PUT: api/produtos/all

        [HttpPut("all")]

        public async Task<IActionResult> UpdateProdutos([FromBody] List<ProdutoUpdateDto> updatedProdutos)
        {
            var ids = updatedProdutos.Select(p => p.Id).ToList();

            var databaseProdutos = await _context.Produtos
                .Where(p => ids.Contains(p.Id) && p.DeletedAt == null)
                .ToListAsync();


            if (!databaseProdutos.Any())
            {
                return NotFound("Nenhum produto localizado com os id's fornecidos.");
            }

            bool globalHasChanges = false;


            foreach (var updated in updatedProdutos)
            {

                var exist = databaseProdutos.FirstOrDefault(p => p.Id == updated.Id);
                bool hasChanges = false;

                bool UpdateIfChanged(string original, string updated, Action<string> setter)
                {
                    var originalTrimmed = original?.Trim() ?? string.Empty;
                    var updatedTrimmed = updated?.Trim() ?? string.Empty;

                    if (originalTrimmed != updatedTrimmed)
                    {
                        setter(updated);
                        return true;
                    }
                    return false;
                }

                bool TecnologiasChanged(string originalString, List<string> updatedArray)
                {
                    var originalList = originalString?
                        .Split(',')
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrEmpty(t))
                        .ToList() ?? new List<string>();

                    var sortedOriginal = originalList.OrderBy(t => t).ToList();
                    var sortedUpdated = updatedArray.OrderBy(t => t).ToList();

                    return !sortedOriginal.SequenceEqual(sortedUpdated);
                }

                hasChanges |= UpdateIfChanged(exist.Nome, updated.Nome, value => exist.Nome = value);
                hasChanges |= UpdateIfChanged(exist.Descricao, updated.Descricao, value => exist.Descricao = value);
                hasChanges |= UpdateIfChanged(exist.FileUrl, updated.FileUrl, value => exist.FileUrl = value);
                hasChanges |= UpdateIfChanged(exist.Instituicao, updated.Instituicao, value => exist.Instituicao = value);
                hasChanges |= UpdateIfChanged(exist.Periodo, updated.Periodo, value => exist.Periodo = value);
                hasChanges |= TecnologiasChanged(exist.Tecnologias, updated.Tecnologias);

                if (hasChanges)
                {
                    exist.Tecnologias = string.Join(", ", updated.Tecnologias);
                }

                if (exist.Tipo != updated.Tipo)
                {
                    exist.Tipo = updated.Tipo;
                    hasChanges = true;
                }

                if (exist.Valor != updated.Valor)
                {
                    exist.Valor = updated.Valor;
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    exist.UpdatedAt = DateTime.UtcNow;
                    globalHasChanges = true;
                }
            }

            if (globalHasChanges)
            {
                await _context.SaveChangesAsync();
                return Ok("Produtos atualizados com sucesso.");
            }

            return Ok("Nenhuma alteração detectada.");
        }

        //DELETE - DELETE: api/produtos/{id}

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProdutoById(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            produto.DeletedAt = DateTime.UtcNow;
            produto.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            //return Ok("Produto exclu�do com sucesso." );
            return Ok(new { mensagem = "Produto exclu�do com sucesso." });
        }
    }
}