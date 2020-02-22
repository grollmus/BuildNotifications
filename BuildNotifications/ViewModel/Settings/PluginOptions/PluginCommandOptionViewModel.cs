﻿using System.Windows.Input;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginCommandOptionViewModel : PluginOptionViewModel
    {
        public PluginCommandOptionViewModel(ICommandOption model, ILocalizationProvider localizationProvider)
            : base(model, localizationProvider)
        {
            Command = new DelegateCommand(model.Execute, model.CanExecute);
        }

        public ICommand Command { get; }
    }
}