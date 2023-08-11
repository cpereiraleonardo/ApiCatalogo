using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Repository;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Categoria>> GetCategorias(CategoriasParemeters categoriasParemeters)
    {
        return await PagedList<Categoria>
         .ToPagedList(Get()
         .OrderBy(on => on.CategoriaId),
         categoriasParemeters.PageNumber,
         categoriasParemeters.PageSize);
    }

    public async Task<PagedList<Categoria>> GetCategoriasProduto(CategoriasParemeters categoriasParemeters)
    {
        return await PagedList<Categoria>
         .ToPagedList(Get()
         .Include(x => x.Produtos)
         .OrderBy(on => on.CategoriaId),
         categoriasParemeters.PageNumber,
         categoriasParemeters.PageSize);
    }
}