using ApiCatalogo.DTOs;
using ApiCatalogo.Filter;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[EnableQuery]
public class ProdutosController : ControllerBase
{
    private string menssagem = "Ocorreu um erro na operação! Segue o erro: ";

    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;


    public ProdutosController(IUnitOfWork context, IMapper mapper)
    {
        _uof = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Método para obter todos os dados de produtos
    /// </summary>
    /// <param name="produtosParameters">Pametros de paginação </param>
    /// <returns>retorna uma lista de produtos</returns>
    [HttpGet]
    [ServiceFilter(typeof(AppiLogginFilter))]
    [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
    {
        try
        {

            var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);

            var metadata = new 
            { 
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentePage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevius
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }

    }

    [HttpGet("produtosCategoria/{id:int}")]
    [ServiceFilter(typeof(AppiLogginFilter))]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorCategoria(int id, [FromQuery]ProdutosParameters produtosParameters)
    {
        try
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorCategoria(id, produtosParameters);

            if (produtos is null)
            {
                return NotFound("Produtos não encontrado!");
            }

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }

    }

    [HttpGet("menorpreco")]
    [ServiceFilter(typeof(AppiLogginFilter))]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>>GetProdutosPrecos()
    {
        
        try
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco();
            var produtosDto =  _mapper.Map<List<ProdutoDTO>>(produtos);

            if (produtos is null)
            {
                return NotFound("Produtos não encontrado!");
            }
            return produtosDto;
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }

    }

    [HttpGet("{id:int}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id) 
    {
        var produto = new Produto();
        try
        {
            produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound($"Produto não encontrado com o id= {id} informado!");
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return produtoDto;
        }
        catch (Exception ex)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }
        
    }

    [HttpPost]
    public async Task<ActionResult> Post(ProdutoDTO produtoDto) 
    {
        var produto = _mapper.Map<Produto>(produtoDto);
        try
        {
            if (produto is null)
            {
                return BadRequest("Erro ao informar dados do produto!");
            }
            _uof.ProdutoRepository.Add(produto);
            await _uof.commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
            /*
            Funciona também dessa forma sem criar uma rota e informar no Protocolo Get um nome
            return CreatedAtAction(nameof(Get), 
                new { id = produto.ProdutoId }, produto););
            */

            return new CreatedAtRouteResult("ObterProduto",
                new { id = produtoDTO.ProdutoId }, produtoDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));

        }
       
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, ProdutoDTO produtoDto) 
    {
        try
        {
            if (id != produtoDto.ProdutoId)
            {
                return BadRequest($"Id= {id} informado não pertence a nenhum produto existente!");
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Update(produto);
            await _uof.commit();

            return Ok();
        }
        catch (Exception ex)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }
        
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id) 
    {
        try
        {
            var produto = await _uof.ProdutoRepository.GetById(p =>
            p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound($"Produto não localizado com id= {id} informado!");
            }

            _uof.ProdutoRepository.Delete(produto);
            await _uof.commit();

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, string.Concat(menssagem, ex.Message));
        }
        
    }
}

