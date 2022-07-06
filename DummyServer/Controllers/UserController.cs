using System.Collections.Generic;
using BuildNotifications.Plugin.DummyServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController(IDataStorage dataStorage)
    {
        _dataStorage = dataStorage;
    }

    [HttpDelete]
    public IActionResult Delete(string name)
    {
        _dataStorage.DeleteUser(name);
        return Ok();
    }

    [HttpGet]
    public ActionResult<IEnumerable<User>> Get() => _dataStorage.Users();

    [HttpPost]
    public IActionResult Push([FromBody] string name)
    {
        _dataStorage.AddUser(name);
        return Ok();
    }

    private readonly IDataStorage _dataStorage;
}