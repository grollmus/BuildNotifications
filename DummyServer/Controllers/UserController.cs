using System.Collections.Generic;
using BuildNotifications.Plugin.DummyServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController
    {
        private readonly IDataStorage _dataStorage;

        public UserController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _dataStorage.Users();
        }

        [HttpPost]
        public void Push([FromBody] string name)
        {
            _dataStorage.AddUser(name);
        }

        [HttpDelete]
        public void Delete(string name)
        {
            _dataStorage.DeleteUser(name);
        }
    }
}