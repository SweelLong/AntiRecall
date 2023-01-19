using System;
using System.Collections.Generic;
using System.Text;
using AntiRecall;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PluginCore;

namespace AntiRecall.Controllers
{
    [Route("api/plugins/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {

        public ActionResult Get()
        {
            SettingsModel settingsModel = PluginSettingsModelFactory.Create<SettingsModel>(nameof(AntiRecall));
            string str = $"Hello PluginCore ! ";

            return Ok(str);
        }

    }
}
