using ApiCatalogo.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiCatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AutorizaController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly IConfiguration? _configuration;

    public AutorizaController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;

    }

    [HttpGet]
    public ActionResult<string> Get() 
    {
        return string.Concat(
            "AutorizaController :: Acessado em : ", DateTime.Now.ToShortTimeString());
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RegisterUser(UsuarioDTO model)
    {
        //if (ModelState.IsValid) 
        //{ 
        //    return BadRequest(ModelState.Values.SelectMany(e => e.Errors));
        //}

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _signInManager.SignInAsync(user, false);

        return Ok(GeraToken(model));
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Login(UsuarioDTO userInfo)
    {
        var result = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Login Inválido!");

            return BadRequest(ModelState);
        }

        return Ok(GeraToken(userInfo));
    }
    
    private UsuarioTokenDTO GeraToken(UsuarioDTO userInfo)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
            new Claim("testeDeClaim", "OpaTesteDeClaim"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expireHours = _configuration["TokenConfiguration:ExpireHours"];

        var expiration = DateTime.UtcNow.AddHours(double.Parse(expireHours));

        JwtSecurityToken token = new(
            issuer: _configuration["TokenConfiguration:Issuer"],
            audience: _configuration["TokenConfiguration:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credenciais);


        return new UsuarioTokenDTO()
        {
            Authenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration,
            Message = "Token JWT OK"
        };

    }

}
