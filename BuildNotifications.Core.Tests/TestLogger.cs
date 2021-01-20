using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace BuildNotifications.Core.Tests
{
    internal class TestLogger : IDisposable
    {
        public TestLogger()
        {
            _target = new MemoryTarget(Guid.NewGuid().ToString());

            var configuration = LogManager.Configuration ?? new LoggingConfiguration();

            configuration.AddTarget(_target.Name, _target);
            configuration.AddRuleForAllLevels(_target);

            LogManager.Configuration = configuration;
        }

        public IEnumerable<string> Messages => _target.Logs.ToList();

        public void Dispose()
        {
            var configuration = LogManager.Configuration;
            configuration.RemoveTarget(_target.Name);
            LogManager.Configuration = configuration;

            _target.Dispose();
        }

        private readonly MemoryTarget _target;
    }
}