using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Http;

namespace ApiCatalogo.Controllers;

//[Authorize(AuthenticationSchemes = "Bearer")]
[Route("api/[controller]")]
[ApiController]
//[EnableCors("PermitirApiRequest")]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly string menssagem = "Ocorreu um erro na operação! Segue o erro: ";

    private readonly IUnitOfWork _uof;

    //private readonly IConfiguration _configuration;

    //private readonly ILogger _logger;

    private readonly IMapper _mapper;

    //public CategoriasController(IUnitOfWork context, IConfiguration configuration, ILogger<CategoriasController> logger, IMapper mapper)
    public CategoriasController(IUnitOfWork context, IMapper mapper)
    {
        _uof = context;
        //_configuration = configuration;
        //_logger = logger;
        _mapper = mapper;
    }

    //******Aqui foi um teste para usar a interface IConfiguration para pergar configuração do arquivo appsettings.json
    //[HttpGet("Autor")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    //public string GetAutor()
    //{
    //    var autor = _configuration["autor"];
    //    var conexao = _configuration["ConnectionStrings:DefaultConnection"];

    //    return $"Autor : {autor} e a conexão de banco é: {conexao}";
    //}

    /// <summary>
    /// Método para obter as categorias por produto
    /// </summary>
    /// <param name="categoriasParameters">recebe os paramentros de paginação</param>
    /// <returns>retorna uma lista de categorias por produtos</returns>
    /// <remarks>retorna uma lista de categorias por produtos</remarks>
    [HttpGet("produtos")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task< ActionResult<IEnumerable<CategoriaDTO>>>GetCategoriasProduto([FromQuery] CategoriasParemeters categoriasParameters)
    {
        try
        {
            //_logger.LogInformation("=================== Get cartegoria/produtos ===============");

            var categorias = await _uof.CategoriaRepository.GetCategoriasProduto(categoriasParameters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentePage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevius
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return categoriasDto;
        }
        catch (Exception ex)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }
    }


    /// <summary>
    /// Método para consultar todas as categorias
    /// </summary>
    /// <param name="categoriasParemeters">paramentros de paginação</param>
    /// <returns>retorna uma lista de categorias</returns>
    /// <remarks>retorna uma lista de categorias</remarks>
    [HttpGet]
    //[EnableCors("PermitirApiRequest")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery]CategoriasParemeters categoriasParemeters)
    {
        try
        {
            //_logger.LogInformation(" =================== Get cartegorias/ ===============");

            var categorias = await _uof.CategoriaRepository.GetCategorias(categoriasParemeters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentePage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevius
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

            return categoriasDto;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }
    }

    /// <summary>
    /// Retornar uma lista de categorias com o ID informado
    /// </summary>
    /// <param name="id">id da categoria</param>
    /// <returns>retorna a categoria do ID informado</returns>
    [HttpGet("{id:int}", Name ="ObterCategoria")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    //[EnableCors("PermitirApiRequest")]
    public async Task<ActionResult<Categoria>> Get(int id) 
    {
        var categoria = new Categoria();
        try
        {
            //_logger.LogInformation($" ===================get cartegorias/id = {id} ===============");
            categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            if (categoria is null)
            {

              //  _logger.LogInformation($" ===================get cartegorias/id = {id} NotFound ===============");

                return NotFound("Categoria não encontrada!");

            }

            return Ok(categoriaDto);
        }
        catch (Exception ex)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }

    }

    /// <summary>
    /// Método para incluir dados de Categorias
    /// </summary>
    /// <param name="categoriaDto">dados da cartegoria (DTO)</param>
    /// <returns>retorna o objeto da categoria incluida</returns>
    /// <remarks>retorna o objeto da categoria incluida</remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Post(CategoriaDTO categoriaDto)
    {
        var categoria = _mapper.Map<Categoria>(categoriaDto);

        try
        {
            if (categoria is null)
            {
                return BadRequest("Dados de categorias não foram informados corretamente!");
            }

            _uof.CategoriaRepository.Add(categoria);
            await _uof.commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoriaDTO.CategoriaId }, categoriaDTO);
        }
        catch (Exception ex)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }
        
    }


    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> Put(int id, CategoriaDTO categoriaDto)
    {
        try
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest($"Não existe categoria para o Id= {id} informado!");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Update(categoria);
            await _uof.commit();

            return Ok(categoriaDto);
        }
        catch (Exception ex)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }
        
    }

    [HttpDelete("{id:int}")]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var categoria = await _uof.CategoriaRepository.GetById(p => (p.CategoriaId == id));

            if (categoria is null)
            {
                return NotFound($"Não foi encontrada categoira com o id: {id} informado!");
            }
            _uof.CategoriaRepository.Delete(categoria);
            await _uof.commit();

            return Ok();
        }
        catch (Exception ex)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }
        
    }
}