using Microsoft.AspNetCore.Mvc;

namespace WeatherApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestErrorsController : ControllerBase
{
    private readonly ILogger<TestErrorsController> _logger;

    public TestErrorsController(ILogger<TestErrorsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Wywołano endpoint testerrors");
        throw new Exception("Testowy wyjątek");
    }
}