﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using ReflectSettings.Attributes;

namespace BuildNotifications.ViewModel.Settings
{
    public class ConnectionsWrapperViewModel
    {
        private readonly IList<ConnectionData> _originalList;
        private readonly IConfiguration _configuration;
        private readonly IPluginRepository _pluginRepository;

        public ConnectionsWrapperViewModel(IList<ConnectionData> originalList, IConfiguration configuration, IPluginRepository pluginRepository)
        {
            _originalList = originalList;
            _configuration = configuration;
            _pluginRepository = pluginRepository;
            var wrappedConnections = originalList.Select(x => new ConnectionDataViewModel(x, _pluginRepository));
            Connections = new ObservableCollection<ConnectionDataViewModel>(wrappedConnections);
            Connections.CollectionChanged += OnCollectionChanged;

            foreach (var connection in Connections)
            {
                connection.TestFinished += InvokeTestFinished;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var connection in e.NewItems.OfType<ConnectionDataViewModel>())
                    {
                        connection.PluginRepository = _pluginRepository;
                        connection.TestFinished += InvokeTestFinished;
                        var wrappedModel = connection.Connection;
                        _originalList.Add(wrappedModel!);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var connection in e.OldItems.OfType<ConnectionDataViewModel>())
                    {
                        connection.TestFinished -= InvokeTestFinished;
                        var wrappedModel = connection.Connection;
                        if (_originalList.Contains(wrappedModel!))
                            _originalList.Remove(wrappedModel!);
                    }

                    break;
            }
        }

        private void InvokeTestFinished(object? sender, EventArgs e) => TestFinished?.Invoke(this, EventArgs.Empty);

        public event EventHandler TestFinished;
        
        [CalculatedValues(nameof(PossibleBuildPlugins), nameof(PossibleBuildPlugins))]
        [CalculatedValues(nameof(PossibleSourceControlPlugins), nameof(PossibleSourceControlPlugins))]
        [CalculatedType(nameof(BuildPluginConfigurationType), nameof(BuildPluginConfigurationType))]
        [CalculatedType(nameof(SourceControlPluginConfigurationType), nameof(SourceControlPluginConfigurationType))]
        public ObservableCollection<ConnectionDataViewModel> Connections { get; set; }
        
        public IEnumerable<string?> PossibleBuildPlugins() => _configuration.PossibleBuildPlugins();

        public IEnumerable<string?> PossibleSourceControlPlugins() => _configuration.PossibleSourceControlPlugins();
        
        public Type BuildPluginConfigurationType(ConnectionData connectionData) => _configuration.BuildPluginConfigurationType(connectionData);

        public Type SourceControlPluginConfigurationType(ConnectionData connectionData) => _configuration.SourceControlPluginConfigurationType(connectionData);
    }
}