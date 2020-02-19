using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Newtonsoft.Json;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    internal class TfsConfiguration : IConfiguration
    {
        public TfsConfiguration()
        {
            Localizer = new TfsLocalizer();

            Url = new TextOption(string.Empty, TextIds.UrlName, TextIds.UrlDescription);
            CollectionName = new TextOption(string.Empty, TextIds.CollectionNameName, TextIds.CollectionNameDescription);
            Project = new ProjectOption();
            Repository = new RepositoryOption();
            AuthenticationType = new EnumOption<AuthenticationType>(Tfs.AuthenticationType.Windows, TextIds.AuthenticationTypeName, TextIds.AuthenticationTypeDescription);
            UserName = new TextOption(string.Empty, TextIds.UserNameName, TextIds.UserNameDescription);
            Password = new EncryptedTextOption(string.Empty, TextIds.PasswordName, TextIds.PasswordDescription);
            Token = new EncryptedTextOption(string.Empty, TextIds.TokenName, TextIds.TokenDescription);

            Url.ValueChanged += OptionChanged;
            CollectionName.ValueChanged += OptionChanged;

            AuthenticationType.ValueChanged += AuthenticationType_ValueChanged;
        }

        public EnumOption<AuthenticationType> AuthenticationType { get; }
        public TextOption CollectionName { get; }
        public EncryptedTextOption Password { get; }
        public ProjectOption Project { get; }
        public RepositoryOption Repository { get; }
        public EncryptedTextOption Token { get; }
        public TextOption Url { get; }
        public TextOption UserName { get; }

        public TfsConfigurationRawData AsRawData()
        {
            return new TfsConfigurationRawData
            {
                Url = Url.Value,
                CollectionName = CollectionName.Value ?? string.Empty,
                Project = Project.Value,
                Repository = Repository.Value,
                AuthenticationType = AuthenticationType.Value,
                Username = UserName.Value,
                Password = Password.Value,
                Token = Token.Value
            };
        }

        private void AuthenticationType_ValueChanged(object? sender, ValueChangedEventArgs<AuthenticationType> e)
        {
            Token.IsVisible = e.NewValue == Tfs.AuthenticationType.Token;
            UserName.IsVisible = e.NewValue == Tfs.AuthenticationType.Account;
            Password.IsVisible = e.NewValue == Tfs.AuthenticationType.Account;
        }

        private async void OptionChanged(object? sender, ValueChangedEventArgs<string?> e)
        {
            var raw = AsRawData();

            var projectTask = Project.FetchAvailableProjects(raw);
            var repositoryTask = Repository.FetchAvailableRepositories(raw);

            await Task.WhenAll(projectTask, repositoryTask);
        }

        public ILocalizer Localizer { get; }

        public bool Deserialize(string serialized)
        {
            try
            {
                var rawData = JsonConvert.DeserializeObject<TfsConfigurationRawData>(serialized);

                Url.Value = rawData.Url;
                CollectionName.Value = rawData.CollectionName;
                Project.Value = rawData.Project;
                Repository.Value = rawData.Repository;
                AuthenticationType.Value = rawData.AuthenticationType;
                UserName.Value = rawData.Username;
                Password.Value = rawData.Password;
                Token.Value = rawData.Token;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public IEnumerable<IOption> ListAvailableOptions()
        {
            yield return Url;
            yield return CollectionName;
            yield return Project;
            yield return Repository;
            yield return AuthenticationType;
            yield return UserName;
            yield return Password;
            yield return Token;
        }

        public string Serialize()
        {
            var raw = AsRawData();

            return JsonConvert.SerializeObject(raw);
        }
    }
}