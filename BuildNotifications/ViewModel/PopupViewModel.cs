using System;

namespace BuildNotifications.ViewModel;

internal abstract class PopupViewModel : BaseViewModel, IRequestClose
{
    protected void RequestClose()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? CloseRequested;
}