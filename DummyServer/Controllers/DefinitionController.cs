using System.Collections.Generic;
using BuildNotifications.Plugin.DummyServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers;

[ApiController]
[Route("[controller]")]
public class DefinitionController : ControllerBase
{
    public DefinitionController(IDataStorage dataStorage)
    {
        _dataStorage = dataStorage;
    }

    [HttpDelete]
    public IActionResult Delete(string name)
    {
        _dataStorage.DeleteDefinition(name);
        return Ok();
    }

    [HttpGet]
    public ActionResult<IEnumerable<BuildDefinition>> Get() => _dataStorage.BuildDefinitions();

    [HttpPost]
    public IActionResult Push([FromBody] string name)
    {
        _dataStorage.AddDefinition(name);
        return Ok();
    }

    private readonly IDataStorage _dataStorage;
}