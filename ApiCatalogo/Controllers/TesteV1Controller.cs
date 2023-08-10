using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers;

//[ApiVersion("1.0", Deprecated =true)]
//[Route("api/v{v:apiVersion}/teste")]
//[ApiController]
public class TesteV1Controller : ControllerBase
{ 
    [HttpGet]
    public IActionResult Get()
    {
        return Content("<html><body><h2>TesteV1Controller - V 1.0</h2></body></html>", "text/html");
    }
}
