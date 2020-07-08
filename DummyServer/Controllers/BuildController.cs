using System.Collections.Generic;
using BuildNotifications.Plugin.DummyBuildServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BuildController : ControllerBase
    {
        private readonly IDataStorage _dataStorage;

        public BuildController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        [HttpGet]
        public IEnumerable<Build> Get()
        {
            return _dataStorage.Builds();
        }
    }
}
