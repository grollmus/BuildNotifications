using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Plugin.DummyServer;
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
        public IEnumerable<WebBuild> Get()
        {
            return _dataStorage.Builds().Select(b => new WebBuild(b));
        }

        [HttpGet]
        [Route(nameof(Extern))]
        public IEnumerable<Build> Extern()
        {
            return _dataStorage.Builds();
        }

        [HttpPost]
        public IActionResult Post([FromBody] WebBuild webBuild)
        {
            var existingBuild = _dataStorage.Builds().FirstOrDefault(b => b.Id.Equals(webBuild.Id));
            if (existingBuild == null)
                _dataStorage.AddBuild(webBuild.BranchName, webBuild.DefinitionName, webBuild.UserName);
            else
            {
                existingBuild.Status = webBuild.Status;
                existingBuild.Reason = webBuild.Reason;
                existingBuild.LastChangedTime = DateTime.Now;
            }

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            _dataStorage.DeleteBuild(id);
            return Ok();
        }

        [HttpPost]
        [Route(nameof(Permutate))]
        public IActionResult Permutate()
        {
            _dataStorage.PermutateBuilds();
            return Ok();
        }

        [HttpPost]
        [Route(nameof(RandomizeBuildStatus))]
        public IActionResult RandomizeBuildStatus()
        {
            _dataStorage.RandomizeBuildStatus();
            return Ok();
        }
    }
}