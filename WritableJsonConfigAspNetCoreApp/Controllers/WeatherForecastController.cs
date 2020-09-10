using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WritableJsonConfigAspNetCoreApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public IWritableOptions2<MySettings> Settings { get; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        //private readonly IWritableOptions<MySettings> _myWritableOptionsAccessor;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config, IWritableOptions2<MySettings> settings)
        {
            Settings = settings;
            _logger = logger;
            _config = config;
            //_myWritableOptionsAccessor = myWritableOptionsAccessor;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            var settings = Settings.Value;
            settings.MyOption = "new value";
            Settings.Update(settings);
            return Settings.Value;
            //await _myWritableOptionsAccessor.Update(x => x.MyOption = "wololo2");
            //return _myWritableOptionsAccessor.Value;
        }

        [HttpPost]
        public IConfiguration Post()
        {
            return _config;
        }
    }
}
