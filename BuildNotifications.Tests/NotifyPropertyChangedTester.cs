using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace BuildNotifications.Tests;

internal class NotifyPropertyChangedTester
{
    public NotifyPropertyChangedTester(INotifyPropertyChanged subject)
    {
        subject.PropertyChanged += OnSubjectPropertyChanged;
    }

    public void AssertFired(params string[] expectedPropertyNames)
    {
        foreach (var propertyName in expectedPropertyNames)
        {
            var contains = _changedPropertyNames.Contains(propertyName);
            Assert.True(contains, $"Expected property {propertyName} not found in changed properties");
        }
    }

    public void AssertFiredOnly(params string[] expectedPropertyNames)
    {
        foreach (var propertyName in expectedPropertyNames)
        {
            var contains = _changedPropertyNames.Contains(propertyName);
            Assert.True(contains, $"Expected property {propertyName} not found in changed properties");
        }

        var notExpected = _changedPropertyNames.Except(expectedPropertyNames);
        Assert.Empty(notExpected);
    }

    public void AssertNotFired(IEnumerable<string> notExpectedPropertyNames)
    {
        foreach (var propertyName in notExpectedPropertyNames)
        {
            var contains = _changedPropertyNames.Contains(propertyName);
            Assert.False(contains, $"Not expected property {propertyName} found in changed properties.");
        }
    }

    private void OnSubjectPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        _changedPropertyNames.Add(e.PropertyName);
    }

    private readonly List<string> _changedPropertyNames = new();
}