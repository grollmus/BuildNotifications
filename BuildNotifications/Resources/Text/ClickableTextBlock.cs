using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BuildNotifications.Resources.Text;

internal class ClickableTextBlock : TextBlock
{
    public ICommand? Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);

        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }

    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
        "CommandParameter", typeof(object), typeof(ClickableTextBlock), new PropertyMetadata(default(object)));

    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        "Command", typeof(ICommand), typeof(ClickableTextBlock), new PropertyMetadata(default(ICommand)));
}