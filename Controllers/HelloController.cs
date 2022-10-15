using Microsoft.AspNetCore.Mvc;

namespace AntiRecall.Controllers
{
    [Route("api/plugins/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        public ActionResult Get()
        {
            string str = $"Hello AntiRecall ! ";
            return Ok(str);
        }
    }
}