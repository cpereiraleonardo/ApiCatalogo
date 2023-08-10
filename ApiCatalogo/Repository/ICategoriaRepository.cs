using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repository;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<PagedList<Categoria>> GetCategoriasProduto(CategoriasParemeters categoriasParemeters);

    Task<PagedList<Categoria>> GetCategorias(CategoriasParemeters categoriasParemeters);
}
