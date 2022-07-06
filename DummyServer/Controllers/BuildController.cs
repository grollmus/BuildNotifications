using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Plugin.DummyServer;
using DummyServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DummyServer.Controllers;

[ApiController]
[Route("[controller]")]
public class BuildController : ControllerBase
{
    public BuildController(IDataStorage dataStorage)
    {
        _dataStorage = dataStorage;
    }

    [HttpDelete]
    public IActionResult Delete(string id)
    {
        _dataStorage.DeleteBuild(id);
        return Ok();
    }

    [HttpGet]
    [Route(nameof(Extern))]
    public IEnumerable<Build> Extern() => _dataStorage.Builds();

    [HttpGet]
    public IEnumerable<WebBuild> Get()
    {
        return _dataStorage.Builds().Select(b => new WebBuild(b));
    }

    [HttpPost]
    [Route(nameof(Permutate))]
    public IActionResult Permutate()
    {
        _dataStorage.PermutateBuilds();
        return Ok();
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

    [HttpPost]
    [Route(nameof(RandomizeBuildStatus))]
    public IActionResult RandomizeBuildStatus()
    {
        _dataStorage.RandomizeBuildStatus();
        return Ok();
    }

    private readonly IDataStorage _dataStorage;
}