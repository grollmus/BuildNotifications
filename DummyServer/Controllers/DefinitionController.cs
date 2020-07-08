using System.Collections.Generic;
using BuildNotifications.Plugin.DummyBuildServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DefinitionController
    {
        private readonly IDataStorage _dataStorage;

        public DefinitionController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        [HttpGet]
        public IEnumerable<BuildDefinition> Get()
        {
            return _dataStorage.BuildDefinitions();
        }

        [HttpPost]
        public void Push([FromBody] string name)
        {
            _dataStorage.AddDefinition(name);
        }

        [HttpDelete]
        public void Delete(string name)
        {
            _dataStorage.DeleteDefinition(name);
        }
    }
}