using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiCatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("INSERT INTO produtos (Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
            "VALUES ('Coca-Cola Diet', 'Refrigerenate de Cola 350 ml', 5.45, 'cocacola.jpg', 50, now(), 1);");

            migrationBuilder.Sql("INSERT INTO produtos (Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
            "VALUES ('Lanche de Atum', 'Lanche de atum com maionese', 8.50, 'atum.jpg', 10, DATE_ADD(now(), INTERVAL -5 DAY), 2);");


            migrationBuilder.Sql("INSERT INTO produtos (Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " + 
            "VALUES ('Pudim', 'Pudim de leite condensado 100g', 6.75, 'pudim.jpg', 20, DATE_ADD(now(), INTERVAL 1 MONTH), 3);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM produtos;");
        }
    }
}
