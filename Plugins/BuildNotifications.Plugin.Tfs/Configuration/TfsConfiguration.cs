using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.PluginInterfaces.Host;
using Newtonsoft.Json;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    internal class TfsConfiguration : AsyncPluginConfiguration
    {
        public TfsConfiguration(IDispatcher uiDispatcher, ConfigurationFlags flags = ConfigurationFlags.None)
            : base(uiDispatcher)
        {
            Localizer = new TfsLocalizer();

            _url = new TextOption(string.Empty, TextIds.UrlName, TextIds.UrlDescription);
            _collectionName = new TextOption(string.Empty, TextIds.CollectionNameName, TextIds.CollectionNameDescription);
            _project = new ProjectOption();
            _repository = new RepositoryOption();
            _authenticationType = new EnumOption<AuthenticationType>(AuthenticationType.Windows, TextIds.AuthenticationTypeName, TextIds.AuthenticationTypeDescription);
            _userName = new TextOption(string.Empty, TextIds.UserNameName, TextIds.UserNameDescription);
            _password = new EncryptedTextOption(string.Empty, TextIds.PasswordName, TextIds.PasswordDescription);
            _token = new EncryptedTextOption(string.Empty, TextIds.TokenName, TextIds.TokenDescription);

            _url.ValueChanged += OptionChanged;
            _collectionName.ValueChanged += OptionChanged;
            _project.ValueChanged += Project_ValueChanged;

            UpdateAuthenticationFieldsVisibility(_authenticationType.Value);
            _authenticationType.ValueChanged += AuthenticationType_ValueChanged;
            _authenticationType.ValueChanged += OptionChanged;

            if (flags.HasFlag(ConfigurationFlags.HideRepository))
                _repository.IsVisible = false;

            var repositoryValueCalculator = CreateCalculator(FetchProjectsAsync, OnProjectsFetched);
            repositoryValueCalculator.Attach(_url);
            repositoryValueCalculator.Attach(_collectionName);
        }

        public override ILocalizer Localizer { get; }

        public TfsConfigurationRawData AsRawData()
        {
            return new TfsConfigurationRawData
            {
                Url = _url.Value,
                CollectionName = _collectionName.Value ?? string.Empty,
                Project = _project.Value,
                Repository = _repository.Value,
                AuthenticationType = _authenticationType.Value,
                Username = _userName.Value,
                Password = _password.Value,
                Token = _token.Value
            };
        }

        public override bool Deserialize(string serialized)
        {
            try
            {
                var rawData = JsonConvert.DeserializeObject<TfsConfigurationRawData>(serialized, new PasswordStringConverter());

                if (rawData != null)
                {
                    _collectionName.Value = rawData.CollectionName;
                    _project.Value = rawData.Project;
                    _repository.Value = rawData.Repository;
                    _authenticationType.Value = rawData.AuthenticationType;
                    _userName.Value = rawData.Username;
                    _password.Value = rawData.Password;
                    _token.Value = rawData.Token;
                    _url.Value = rawData.Url;

                    FetchAvailableValues(rawData);

                    return true;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public override IEnumerable<IOption> ListAvailableOptions()
        {
            yield return _url;
            yield return _collectionName;
            yield return _authenticationType;
            yield return _userName;
            yield return _password;
            yield return _token;
            yield return _project;
            yield return _repository;
        }

        public override string Serialize()
        {
            var raw = AsRawData();

            return JsonConvert.SerializeObject(raw, Formatting.None, new PasswordStringConverter());
        }

        private void AuthenticationType_ValueChanged(object? sender, EventArgs e)
        {
            UpdateAuthenticationFieldsVisibility(_authenticationType.Value);
        }

        private void FetchAvailableValues(TfsConfigurationRawData raw)
        {
            _repository.FetchAvailableRepositories(raw).FireAndForget();
        }

        private async Task<IAsyncValueCalculationResult<IEnumerable<TfsProject>>> FetchProjectsAsync(CancellationToken token)
        {
            try
            {
                var projects = await _project.FetchAvailableProjects(AsRawData());
                return ValueCalculationResult.Success(projects);
            }
            catch (Exception)
            {
                return ValueCalculationResult.Fail<IEnumerable<TfsProject>>();
            }
        }

        private void OnProjectsFetched(IEnumerable<TfsProject> fetchedProjects)
        {
            _project.SetAvailableProjects(fetchedProjects);
        }

        private void OptionChanged(object? sender, EventArgs e)
        {
            var raw = AsRawData();

            FetchAvailableValues(raw);
        }

        private async void Project_ValueChanged(object? sender, EventArgs e)
        {
            var raw = AsRawData();

            await _repository.FetchAvailableRepositories(raw);
        }

        private void UpdateAuthenticationFieldsVisibility(AuthenticationType authenticationType)
        {
            _token.IsVisible = authenticationType == AuthenticationType.Token;
            _userName.IsVisible = authenticationType == AuthenticationType.Account;
            _password.IsVisible = authenticationType == AuthenticationType.Account;
        }

        private readonly EnumOption<AuthenticationType> _authenticationType;
        private readonly TextOption _collectionName;
        private readonly EncryptedTextOption _password;
        private readonly ProjectOption _project;
        private readonly RepositoryOption _repository;
        private readonly EncryptedTextOption _token;
        private readonly TextOption _url;
        private readonly TextOption _userName;
    }
}