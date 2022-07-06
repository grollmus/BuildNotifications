using System.Threading.Tasks;
using System.Windows.Input;

namespace BuildNotifications.ViewModel.Utils;

public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object parameter);
}