using System.Media;
using System.Windows;
using System.Windows.Interop;
using BuildNotifications.Core.Config;
using BuildNotifications.ViewModel;
using BuildNotifications.ViewModel.Settings;
using BuildNotifications.Views;
using BuildNotifications.Views.Settings;
using Microsoft.Win32;

namespace BuildNotifications.Services;

internal class PopupService : IPopupService
{
    public PopupService(IBlurrableViewModel blur, IViewProvider viewProvider)
    {
        _blur = blur;
        _viewProvider = viewProvider;
    }

    private void ShowPopup<TDialog>(object dataContext)
        where TDialog : Window, new()
    {
        var popup = new TDialog
        {
            Owner = _viewProvider.View,
            DataContext = dataContext
        };

        _blur.Blur();
        popup.ShowDialog();
        _blur.UnBlur();
    }

    public MessageBoxResult ShowMessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage icon = MessageBoxImage.Asterisk, MessageBoxResult defaultResult = MessageBoxResult.None)
    {
        var vm = new MessageBoxViewModel(text, title, buttons, icon, defaultResult);

        var owner = _viewProvider.View;
        var popup = new MessageBoxView
        {
            Owner = owner,
            DataContext = vm
        };

        switch (icon)
        {
            case MessageBoxImage.Asterisk:
                SystemSounds.Asterisk.Play();
                break;
            case MessageBoxImage.Error:
                SystemSounds.Exclamation.Play();
                break;
            case MessageBoxImage.Warning:
                SystemSounds.Beep.Play();
                break;
            case MessageBoxImage.Question:
                SystemSounds.Question.Play();
                break;
        }

        var interopWindow = new WindowInteropHelper(owner);
        _ = NativeMethods.FlashWindow(interopWindow.Handle, true);

        _blur.Blur();
        popup.ShowDialog();
        _blur.UnBlur();

        return vm.Result;
    }

    public void ShowInfoPopup(bool includePreReleases, IAppUpdater appUpdater)
    {
        var dataContext = new InfoPopupViewModel(appUpdater, includePreReleases);
        ShowPopup<InfoPopupDialog>(dataContext);
    }

    public string? AskForOpenFileName(params string[] fileTypes)
    {
        var dlg = new OpenFileDialog
        {
            Filter = string.Join("|", fileTypes)
        };
        return dlg.ShowDialog(_viewProvider.View) != true ? null : dlg.FileName;
    }

    public string? AskForSaveFileName(params string[] fileTypes)
    {
        var dlg = new SaveFileDialog
        {
            Filter = string.Join("|", fileTypes)
        };
        return dlg.ShowDialog(_viewProvider.View) != true ? null : dlg.FileName;
    }

    public void ShowImportExportPopup(IConfigurationService configurationService)
    {
        var dataContext = new ImportExportPopupViewModel(this, configurationService);
        ShowPopup<ImportExportDialog>(dataContext);
    }

    private readonly IBlurrableViewModel _blur;
    private readonly IViewProvider _viewProvider;
}