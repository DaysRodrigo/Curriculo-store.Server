using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Curriculo_store.Server.Controllers;
using Curriculo_store.Server.Models;
using Curriculo_store.Server.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Curriculo_store.Server.Data;

public class ProdutosControllerTests
{
    private ApplicationDbContext GetDbContextWithData()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        var context = new ApplicationDbContext(options);

        //Product test
        context.Produtos.AddRange(
            new Produto
            {
                Id = 1,
                Nome = "Produto 1",
                Tipo = TipoProduto.Curso, // ajustar conforme enum
                Descricao = "Descrição do produto 1",
                Instituicao = "Instituição X",
                Valor = 100,
                Periodo = "Mensal",
                Tecnologias = "[\"C#\",\".NET\"]",
                DeletedAt = null
            },
            new Produto
            {
                Id = 2,
                Nome = "Produto 2",
                Tipo = TipoProduto.Curso,
                Descricao = "Descrição do produto 2",
                Instituicao = "Instituição Y",
                Valor = 200,
                Periodo = "Semestral",
                Tecnologias = "[\"React\",\"TypeScript\"]",
                DeletedAt = null
            },
            new Produto
            {
                Id = 3,
                Nome = "Produto 3",
                Tipo = TipoProduto.Curso,
                Descricao = "Descrição do produto 3",
                Instituicao = "Instituição Z",
                Valor = 300,
                Periodo = "Anual",
                Tecnologias = "[\"Java\",\"Spring\"]",
                DeletedAt = DateTime.UtcNow // deve ser ignorado
            }
        );
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetAllProdutos_ShouldReturnOnlyNonDeletedProducts()
    {
        //Arrange
        var context = GetDbContextWithData();
        var controller = new ProdutosController(context);


        //Act
        var result = await controller.GetAllProdutos();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var produtos = okResult.Value as List<Produto>;
        produtos.Should().HaveCount(2); // só os não excluídos
        produtos.Should().Contain(produtos => produtos.Nome == "Produto 1");
        produtos.Should().Contain(produtos => produtos.Nome == "Produto 2");
    }

    [Fact]
    public async Task GetAllProdutos_ShouldReturnNoContent_WhenNoProducts()
    {
        //Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "EmptyDb")
            .Options;

        var context = new ApplicationDbContext(options);
        var controller = new ProdutosController(context);

        //Act
        var result = await controller.GetAllProdutos();

        //Assert
        var noContentResult = result.Result as NoContentResult;
        noContentResult.Should().NotBeNull();
    }
}