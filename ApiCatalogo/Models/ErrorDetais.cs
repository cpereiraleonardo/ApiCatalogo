using Newtonsoft.Json;

namespace ApiCatalogo.Models;

public class ErrorDetais
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Trace { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

}
