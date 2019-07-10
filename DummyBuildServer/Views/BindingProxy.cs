using System.Windows;
using DummyBuildServer.ViewModels;

namespace DummyBuildServer.Views
{
    internal class GenericBindingProxy<TData> : Freezable
        where TData : class
    {
        public TData Data
        {
            get => (TData) GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GenericBindingProxy<TData>();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(TData),
            typeof(GenericBindingProxy<TData>),
            new UIPropertyMetadata(null));
    }

    internal class BindingProxy : GenericBindingProxy<MainViewModel>
    {
    }
}