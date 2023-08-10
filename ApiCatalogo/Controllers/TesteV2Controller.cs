using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers;

//[ApiVersion("2.0")]
///[Route("api/v{v:apiVersion}/teste")]
//[ApiController]
public class TesteV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Content("<html><body><h2>TesteV2Controller - V 2.0</h2></body></html>", "text/html");
    }
}
