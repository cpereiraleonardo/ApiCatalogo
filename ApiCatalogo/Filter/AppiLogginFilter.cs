using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiCatalogo.Filter;

public class AppiLogginFilter : IActionFilter
{
    private readonly ILogger<AppiLogginFilter> _logger;

    public AppiLogginFilter(ILogger<AppiLogginFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("### Excutando -> onActionExecuting ###");
        _logger.LogInformation("#####################################################");
        _logger.LogInformation($"Data de Excução: {DateTime.Now.ToLongTimeString()}");
        _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
        _logger.LogInformation("#####################################################");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("### Excutado -> onActionExecuted ###");
        _logger.LogInformation("#####################################################");
        _logger.LogInformation($"Data de Excução: {DateTime.Now.ToLongTimeString()}");
        _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
        _logger.LogInformation("#####################################################");
    }
   
}
