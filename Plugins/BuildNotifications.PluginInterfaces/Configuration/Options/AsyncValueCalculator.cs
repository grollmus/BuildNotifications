using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Host;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    internal class AsyncValueCalculator<T> : IAsyncValueCalculator
    {
        public AsyncValueCalculator(IDispatcher dispatcher, Func<CancellationToken, Task<IValueCalculationResult<T>>> calculationTaskFactory, Action<T> handleResultCallback)
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

        private async Task ExecuteUpdate(Task<IValueCalculationResult<T>> task, CancellationToken token)
        {
            SetLoadingFlag(true);

            try
            {
                var result = await task;
                if (result.Success && !token.IsCancellationRequested)
                    _dispatcher.Dispatch(() => _handleResultCallback(result.Value));
            }
            catch (TaskCanceledException)
            {
                // ignored because a new update has been started
            }
            catch
            {
                SetLoadingFlag(false);
            }
            finally
            {
                var newUpdateRunning = token.IsCancellationRequested;
                if (!newUpdateRunning)
                    SetLoadingFlag(false);
            }
        }

        private void OnValueChanged(object? sender, EventArgs e) => Update();

        private void SetLoadingFlag(bool isLoading)
        {
            _dispatcher.Dispatch(() =>
            {
                lock (_syncRoot)
                {
                    foreach (var option in _affectedOptions)
                    {
                        option.IsLoading = isLoading;
                    }
                }
            });
        }

        public void RemoveAffect(params IOption[] options)
        {
            lock (_syncRoot)
            {
                foreach (var option in options)
                {
                    _affectedOptions.Remove(option);
                }
            }
        }

        public void Update()
        {
            ClearAllTokens();
            var token = CreateNewToken();

            Task.Run(() => ExecuteUpdate(_calculationTaskFactory(token), token), token);
        }

        public void Attach(params IValueOption[] options)
        {
            foreach (var option in options)
            {
                option.ValueChanged -= OnValueChanged;
                option.ValueChanged += OnValueChanged;
            }
        }

        public void Detach(params IValueOption[] options)
        {
            foreach (var option in options)
            {
                option.ValueChanged -= OnValueChanged;
            }
        }

        public void Affect(params IOption[] options)
        {
            lock (_syncRoot)
            {
                _affectedOptions.AddRange(options);
            }
        }

        public void Dispose() => ClearAllTokens();

        private readonly object _syncRoot = new object();
        private readonly IDispatcher _dispatcher;
        private readonly Func<CancellationToken, Task<IValueCalculationResult<T>>> _calculationTaskFactory;
        private readonly Action<T> _handleResultCallback;
        private readonly List<IOption> _affectedOptions = new List<IOption>();
        private readonly IList<CancellationTokenSource> _tokenSources = new List<CancellationTokenSource>();
    }
}