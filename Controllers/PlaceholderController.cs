using Microsoft.AspNetCore.Mvc;

namespace app_reclamos_seguros.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaceholderController : ControllerBase
    {
        private readonly ILogger<PlaceholderController> _logger;
        public PlaceholderController(ILogger<PlaceholderController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetPlaceholder")]
        public IEnumerable<object> Get()
        {
            return null;
        }
    }
}
