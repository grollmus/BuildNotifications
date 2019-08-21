using System.Collections.Generic;
using ReflectSettings.Attributes;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    public class Configuration
    {
        [MinMax(0, 99999)]
        public int Port { get; set; } = 1111;

        public bool DemonstrateSylenceDynamicTypeChanges { get; set; }

        [CalculatedValues(nameof(SomeValues))]
        public string SomeString { get; set; }

        public IEnumerable<string> SomeValues()
        {
            if (DemonstrateSylenceDynamicTypeChanges)
            {
                yield return "I'm different :O";
                yield return "I see you";
            }
            else
            {
                yield return "Hello";
                yield return "Bye";
            }
        }
    }
}