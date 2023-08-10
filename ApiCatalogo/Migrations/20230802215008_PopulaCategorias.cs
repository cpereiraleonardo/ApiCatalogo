using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiCatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO categorias (Nome, ImageUrl) VALUES ('Bebidas', 'bebidas.jpg');");
            migrationBuilder.Sql("INSERT INTO categorias(Nome, ImageUrl) VALUES ('Lanches', 'lanches.jpg');");
            migrationBuilder.Sql("INSERT INTO categorias (Nome, ImageUrl) VALUES ('Sobremesas', 'sobremeas.jpg');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM categorias;");
        }
    }
}
