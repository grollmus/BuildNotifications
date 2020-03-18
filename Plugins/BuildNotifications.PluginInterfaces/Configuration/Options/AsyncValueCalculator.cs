using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Host;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    internal class AsyncValueCalculator<T> : IAsyncValueCalculator
    {
        public AsyncValueCalculator(IDispatcher dispatcher, Func<CancellationToken, Task<IAsyncValueCalculationResult<T>>> calculationTaskFactory, Action<T> handleResultCallback)
        {
            _dispatcher = dispatcher;
            _calculationTaskFactory = calculationTaskFactory;
            _handleResultCallback = handleResultCallback;
        }

        private void ClearAllTokens()
        {
            foreach (var token in _tokenSources)
            {
                token.Cancel();
                token.Dispose();
            }

            _tokenSources.Clear();
        }

        private CancellationToken CreateNewToken()
        {
            var newToken = new CancellationTokenSource();
            _tokenSources.Add(newToken);

            return newToken.Token;
        }

        private async Task ExecuteUpdate(Task<IAsyncValueCalculationResult<T>> task)
        {
            try
            {
                var result = await task;
                if (result.Success)
                    _dispatcher.Dispatch(() => _handleResultCallback(result.Value));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void OnValueChanged(object? sender, EventArgs e)
        {
            Update();
        }

        public void Update()
        {
            ClearAllTokens();
            var token = CreateNewToken();

            Task.Run(() => ExecuteUpdate(_calculationTaskFactory(token)), token);
        }

        public void Attach(IValueOption option)
        {
            option.ValueChanged -= OnValueChanged;
            option.ValueChanged += OnValueChanged;
        }

        public void Detach(IValueOption option) => option.ValueChanged -= OnValueChanged;

        public void Dispose() => ClearAllTokens();

        private readonly IDispatcher _dispatcher;
        private readonly Func<CancellationToken, Task<IAsyncValueCalculationResult<T>>> _calculationTaskFactory;
        private readonly Action<T> _handleResultCallback;

        private readonly IList<CancellationTokenSource> _tokenSources = new List<CancellationTokenSource>();
    }

    
}