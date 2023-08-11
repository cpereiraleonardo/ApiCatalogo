using ApiCatalogo.Repository;
using GraphQL;
using GraphQL.Types;
using System.Text.Json;

namespace ApiCatalogo.GraphQL;

public class TesteGraphQLMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IUnitOfWork _context;

    public TesteGraphQLMiddleware(RequestDelegate next, IUnitOfWork context)
    {
        _next = next;
        _context = context;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        //verifica se o caminho do request na URL é /graphql
        if (httpContext.Request.Path.StartsWithSegments("/graphql"))
        {
            //tenta ler o corpo do request usuando StreamReader
            using (var stream = new StreamReader(httpContext.Request.Body))
            {
                //um objeto schema é criado com a propriedade Query e é definida uma instância do contexo (repositório)
                var query = await stream.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    var schema = new Schema
                    {
                        Query = new CategoriaQuery(_context)
                    };

                    //é criado um DocumentExecuter que executa a consulta contra o schema e o resultado é escrito como Json via WriteResult
                    var result = await new DocumentExecuter().ExecuteAsync(options =>
                    {
                        options.Schema = schema;
                        options.Query = query;
                    });
                    await WriteResult(httpContext, result);
                }
            }

        }
        else
        {
            await _next(httpContext);
        }

    }

    private async Task WriteResult(HttpContext httpContext, ExecutionResult result)
    {

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(result, options);
        httpContext.Response.StatusCode = 200;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsync(jsonString);
    }
}
