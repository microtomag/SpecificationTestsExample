using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FunctionsApp;

public class ValidatePerson
{
    private readonly ILogger<ValidatePerson> _logger;

    public ValidatePerson(ILogger<ValidatePerson> logger)
    {
        _logger = logger;
    }

    [Function("ValidatePerson")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(new {IsValid = true});
        
    }

}