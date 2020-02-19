using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Options
{
    internal abstract class OptionViewModel : BaseViewModel
    {
        protected OptionViewModel(IOption option)
        {
            Option = option;
        }

        public string Description => Option.DescriptionTextId;
        public string DisplayName => Option.NameTextId;
        public bool IsEnabled => Option.IsEnabled;
        public bool IsVisible => Option.IsVisible;

        protected IOption Option { get; }
    }

    internal class ValueOptionViewModel<TValue> : OptionViewModel
    {
        public ValueOptionViewModel(ValueOption<TValue> valueOption)
            : base(valueOption)
        {
            ValueOption = valueOption;
            ValueOption.ValueChanged += ValueOption_ValueChanged;
        }

        public TValue Value
        {
            get => ValueOption.Value;
            set => ValueOption.Value = value;
        }

        protected ValueOption<TValue> ValueOption { get; }

        private void ValueOption_ValueChanged(object? sender, ValueChangedEventArgs<TValue> e)
        {
            OnPropertyChanged(nameof(Value));
        }
    }

    internal class TextOptionViewModel : ValueOptionViewModel<string?>
    {
        public TextOptionViewModel(TextOption valueOption)
            : base(valueOption)
        {
        }
    }

    internal class ListOptionViewModel<TValue> : ValueOptionViewModel<TValue>
    {
        public ListOptionViewModel(ListOption<TValue> valueOption) : base(valueOption)
        {
            ListOption = valueOption;
        }

        public IEnumerable<ListOptionItemViewModel<TValue>> AvailableValues => ListOption.AvailableValues.Select(x => new ListOptionItemViewModel<TValue>(x));

        private readonly ListOption<TValue> ListOption;
    }

    internal class ListOptionItemViewModel<TValue> : BaseViewModel
    {
        public ListOptionItemViewModel(ListOptionItem<TValue> item)
        {
            Name = item.DisplayName;
            Value = item.Value;
        }

        public string Name { get; }
        public TValue Value { get; }
    }

    internal class CommandOptionViewModel : OptionViewModel
    {
        public CommandOptionViewModel(ICommandOption model)
            : base(model)
        {
            Command = new DelegateCommand(x => model.Execute(), x => model.CanExecute());
        }

        public ICommand Command { get; }
    }
}