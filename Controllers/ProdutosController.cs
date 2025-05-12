using Curriculo_store.Server.Models;
using Curriculo_store.Server.Shared.Enums;
using Curriculo_store.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> CreateProduto([FromBody] Produto produto)
        {
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
                (produto.Tipo == TipoProduto.Acadêmico && string.IsNullOrWhiteSpace(produto.FileUrl)))
            {
                return BadRequest("Para o tipo Curso, é necessário fornecer um arquivo.");
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction( nameof(GetProdutoById), new { id = produto.Id }, produto);
        }

        //READ ONE - GET: api/produtos/{id}
        [HttpGet("{id}")]

        public async Task<ActionResult<Produto>> GetProdutoById(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if ( produto == null )
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
                .ToListAsync();

            if ( produtos == null || produtos.Count == 0 )
            {
                return NoContent();
            }

            return Ok( produtos );
        }

        //UPDATE BY ID - PUT: api/produtos/{id}

        [HttpPut("{id}")]

        public async Task<ActionResult<Produto>> UpdateProdutoById(int id, [FromBody] Produto updatedProduto)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if ( produto == null )
            {
                return NotFound();
            }

            if (produto.DeletedAt != null)
            {
                return BadRequest("Não é possível atualizar um produto excluído.");
            }


            if ( produto.Nome.Trim() != updatedProduto.Nome.Trim() ||
                produto.Tipo != updatedProduto.Tipo ||
                produto.Descricao.Trim() != updatedProduto.Descricao.Trim() ||
                produto.FileUrl.Trim() != updatedProduto.FileUrl.Trim())
            {
                produto.Nome = updatedProduto.Nome;
                produto.Tipo = updatedProduto.Tipo;
                produto.Descricao = updatedProduto.Descricao;
                produto.FileUrl = updatedProduto.FileUrl;
                produto.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(produto);
        }

        //MASS UPDATE - PUT: api/produtos/all

        [HttpPut("all")]

        public async Task<IActionResult> UpdateProdutos([FromBody] List<Produto> updatedProdutos)
        {
            var ids = updatedProdutos.Select( p =>  p.Id ).ToList();

            var databaseProdutos = await _context.Produtos
                .Where( p => ids.Contains( p.Id ) && p.DeletedAt == null)
                .ToListAsync();


            if (!databaseProdutos.Any())
            {
                return NotFound("Nenhum produto localizado com os id's fornecidos.");
            }

            foreach ( var updated in updatedProdutos )
            {

                var exist = databaseProdutos.FirstOrDefault( p => p.Id == updated.Id );

                if (exist == null) continue;

                    if (exist.Nome.Trim() != updated.Nome.Trim() ||
                        exist.Tipo != updated.Tipo ||
                        exist.Descricao.Trim() != updated.Descricao.Trim() ||
                        exist.FileUrl.Trim() != updated.FileUrl.Trim())
                    {
                        exist.Nome = updated.Nome;
                        exist.Tipo = updated.Tipo;
                        exist.Descricao = updated.Descricao;
                        exist.FileUrl = updated.FileUrl;
                        exist.UpdatedAt = DateTime.UtcNow;
                    }
            }

            await _context.SaveChangesAsync();
            return Ok("Produtos atualizados com sucesso.");
        }

        //DELETE - DELETE: api/produtos/{id}

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProdutoById(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if ( produto == null )
            {
                return NotFound();
            }

            produto.DeletedAt = DateTime.UtcNow;
            produto.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Produto excluído com sucesso.");


        }
    }
}