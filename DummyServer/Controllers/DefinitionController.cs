using System.Collections.Generic;
using BuildNotifications.Plugin.DummyServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefinitionController : ControllerBase
    {
        private readonly IDataStorage _dataStorage;

        public DefinitionController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BuildDefinition>> Get()
        {
            return _dataStorage.BuildDefinitions();
        }

        [HttpPost]
        public IActionResult Push([FromBody] string name)
        {
            _dataStorage.AddDefinition(name);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(string name)
        {
            _dataStorage.DeleteDefinition(name);
            return Ok();
        }
    }
}