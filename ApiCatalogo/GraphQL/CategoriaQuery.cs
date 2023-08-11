using ApiCatalogo.Repository;
using GraphQL;
using GraphQL.Types;

namespace ApiCatalogo.GraphQL;

public class CategoriaQuery : ObjectGraphType
{
    public CategoriaQuery(IUnitOfWork _context)
    {

        ///Método para consultar categorias pelo ID
        Field<CategoriaType>("categoria",
             arguments: new QueryArguments(
                 new QueryArgument<IntGraphType>() { Name = "id" }),
                    resolve: context =>
                        {
                            var id = context.GetArgument<int>("id");
                            return _context.CategoriaRepository.GetById(c => c.CategoriaId == id);
                        }
                 );

        //Método para consultar categorias.
        Field<CategoriaType>("categorias",
            resolve: context =>
            {
                return _context.CategoriaRepository.Get();
            });
    }

}
