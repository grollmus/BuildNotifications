using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Plugin.DummyBuildServer;
using BuildNotifications.PluginInterfaces.Builds;
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

        [HttpPost]
        public void Post([FromBody] WebBuild webBuild)
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
        }

        [HttpDelete]
        public void Delete(string id)
        {
            _dataStorage.DeleteBuild(id);
        }

        [HttpGet]
        [Route(nameof(Permutate))]
        public void Permutate() => _dataStorage.PermutateBuilds();

        [HttpGet]
        [Route(nameof(RandomizeBuildStatus))]
        public void RandomizeBuildStatus() => _dataStorage.RandomizeBuildStatus();
    }

    public class WebBuild
    {
        public WebBuild(Build build)
        {
            BranchName = build.BranchName;
            DefinitionName = build.Definition.Name;
            UserName = build.RequestedBy.UniqueName;
            Id = build.Id;
            Status = build.Status;
            Reason = build.Reason;
        }

        public WebBuild()
        {
            // needed for serialization   
        }

        public string BranchName { get; set; }

        public string DefinitionName { get; set; }

        public string UserName { get; set; }

        public string Id { get; set; }

        public BuildStatus Status { get; set; }

        public BuildReason Reason { get; set; }
    }
}