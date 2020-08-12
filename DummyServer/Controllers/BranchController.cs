using System.Collections.Generic;
using BuildNotifications.Plugin.DummyServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IDataStorage _dataStorage;

        public BranchController(IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Branch>> Get()
        {
            return _dataStorage.Branches();
        }

        [HttpPost]
        public IActionResult Push([FromBody] string name)
        {
            _dataStorage.AddBranch(name);
            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(string name)
        {
            _dataStorage.DeleteBranch(name);
            return Ok();
        }
    }
}