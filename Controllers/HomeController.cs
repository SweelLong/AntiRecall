using Microsoft.AspNetCore.Mvc;
using PluginCore;

namespace AntiRecall.Controllers
{
    [Route("Plugins/AntiRecall")]
    public class HomeController : Controller
    {
        public ActionResult Get()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir("AntiRecall"), "index.html");
            return PhysicalFile(indexFilePath, "text/html");
        }
    }
}