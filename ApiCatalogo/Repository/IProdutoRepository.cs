using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repository;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IEnumerable<Produto>> GetProdutosPorPreco();

    Task<IEnumerable<Produto>> GetProdutosPorCategoria(int id, ProdutosParameters produtosParameters);

    Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters);

}

