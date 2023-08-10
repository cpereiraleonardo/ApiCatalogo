namespace ApiCatalogo.Repository;

public interface IUnitOfWork
{
    public IProdutoRepository ProdutoRepository { get; }

    public ICategoriaRepository CategoriaRepository { get; }

    Task commit();
}
