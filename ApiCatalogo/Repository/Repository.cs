using ApiCatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApiCatalogo.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    protected AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public IQueryable<T> Get()
    {
        return _context.Set<T>().AsNoTracking();
    }

    public async Task<T> GetById(Expression<Func<T, bool>> predicate)
    {
#pragma warning disable CS8603 // Possível retorno de referência nula.
        return await _context.Set<T>().SingleOrDefaultAsync(predicate);
#pragma warning restore CS8603 // Possível retorno de referência nula.
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.Set<T>().Update(entity);
    }
}
