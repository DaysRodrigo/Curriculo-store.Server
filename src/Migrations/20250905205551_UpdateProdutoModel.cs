using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Curriculo_store.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProdutoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instituicao",
                table: "Produtos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Periodo",
                table: "Produtos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tecnologias",
                table: "Produtos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "Valor",
                table: "Produtos",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instituicao",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Tecnologias",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Produtos");
        }
    }
}
