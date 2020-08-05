using System.Collections.Generic;
using BuildNotifications.Plugin.DummyServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BranchController
    {
        private readonly IDataStorage _dataStorage;

        public BranchController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        [HttpGet]
        public IEnumerable<Branch> Get()
        {
            return _dataStorage.Branches();
        }

        [HttpPost]
        public void Push([FromBody] string name)
        {
            _dataStorage.AddBranch(name);
        }

        [HttpDelete]
        public void Delete(string name)
        {
            _dataStorage.DeleteBranch(name);
        }
    }
}