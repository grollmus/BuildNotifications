using System.Collections.Generic;
using BuildNotifications.Plugin.DummyServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IDataStorage _dataStorage;

        public UserController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return _dataStorage.Users();
        }

        [HttpPost]
        public IActionResult Push([FromBody] string name)
        {
            _dataStorage.AddUser(name);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(string name)
        {
            _dataStorage.DeleteUser(name);
            return Ok();
        }
    }
}