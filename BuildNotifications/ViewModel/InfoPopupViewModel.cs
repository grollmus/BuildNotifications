using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Services;
using BuildNotifications.ViewModel.Utils;
using Semver;

namespace BuildNotifications.ViewModel
{
    internal class InfoPopupViewModel : BaseViewModel
    {
        public InfoPopupViewModel(IAppUpdater appUpdater, IConfiguration configuration)
        {
            _appUpdater = appUpdater;
            _configuration = configuration;
            OpenUrlCommand = new DelegateCommand<string>(OpenAboutUrl);
            CheckForUpdatesCommand = AsyncCommand.Create(CheckForUpdatesAsync);
            UpdateCommand = AsyncCommand.Create(UpdateAppAsync);
        }

        public string AppVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";

        public AsyncCommand<bool> CheckForUpdatesCommand { get; }

        public ICommand OpenUrlCommand { get; }

        public IAsyncCommand UpdateCommand { get; }

        private async Task<bool> CheckForUpdatesAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            var result = await _appUpdater.CheckForUpdates();
            if (result == null)
                return false;

            if (!SemVersion.TryParse(result.CurrentVersion, out var currentVersion))
                currentVersion = new SemVersion(0);

            var versions = result.ReleasesToApply.Select(TryParseSemVersion);
            if (!_configuration.UsePreReleases)
                versions = versions.Where(v => string.IsNullOrEmpty(v.Prerelease));

            var newestVersion = versions.OrderByDescending(x => x).FirstOrDefault();
            if (newestVersion != null && newestVersion > currentVersion)
                return true;

            return false;
        }

        private void OpenAboutUrl(object obj)
        {
            var url = obj as string;
            Url.GoTo(url);
        }

        private static SemVersion TryParseSemVersion(ReleaseToApply r)
        {
            return SemVersion.TryParse(r.Version, out var version) ? version : new SemVersion(0);
        }

        private async Task UpdateAppAsync()
        {
            await _appUpdater.PerformUpdate();
        }

        private readonly IAppUpdater _appUpdater;
        private readonly IConfiguration _configuration;
    }
}