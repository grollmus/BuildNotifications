using System;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.PluginInterfaces.Host;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options
{
    public class AsyncValueCalculatorTests
    {
        private IDispatcher CreateDispatcher()
        {
            var dispatcher = Substitute.For<IDispatcher>();

            dispatcher
                .WhenForAnyArgs(x => x.Dispatch(Arg.Any<Action>()))
                .Do(info => info.ArgAt<Action>(0).Invoke());

            return dispatcher;
        }

        [Fact]
        public async Task ChangingValueOfAttachedOptionShouldTriggerUpdate()
        {
            // Arrange
            var tcs = new TaskCompletionSource<int>();
            var dispatcher = CreateDispatcher();

            Task<IValueCalculationResult<int>> CalculationTaskFactory(CancellationToken ct)
            {
                return Task.FromResult<IValueCalculationResult<int>>(ValueCalculationResult.Success(123));
            }

            void HandleResultCallback(int arg)
            {
                tcs.SetResult(arg);
            }

            using var sut = new AsyncValueCalculator<int>(dispatcher, CalculationTaskFactory, HandleResultCallback);

            var option = Substitute.For<IValueOption>();
            sut.Attach(option);

            // Act
            option.ValueChanged += Raise.Event();

            // Assert
            var result = await tcs.Task;
            Assert.Equal(123, result);
        }

        [Fact]
        public async Task MultipleUpdateShouldTriggerCallbackOnlyOnce()
        {
            // As with all async code there is much magic happening here:
            // We want to start a second update while the first one is still running.
            // To do so we start the first update wait for a WaitHandle to be
            // signaled and then start the second update.
            //
            // When the update method is run, it will signal the wait handle
            // then wait some time to let the second update start
            //
            // The callback will set a TaskCompletionSource and this task is awaited
            // in the test.
            //
            // The asserts check that
            // - The update method was called twice
            // - The result callback was only called once
            // - The affected option is not loading anymore
            // - The loading flags was set and unset correctly
            //
            // Yes one assert per test :x

            // Arrange
            var tcs = new TaskCompletionSource<int>();
            var dispatcher = CreateDispatcher();
            var callbackCount = 0;
            var calculationCount = 0;

            var updateStartBlock = new ManualResetEventSlim(false);
            var updateFinishBlock = new ManualResetEventSlim(false);

            async Task<IValueCalculationResult<int>> CalculationTaskFactory(CancellationToken ct)
            {
                Interlocked.Increment(ref calculationCount);
                updateStartBlock.Set();
                await Task.Delay(25, ct);
                return ValueCalculationResult.Success(123);
            }

            void HandleResultCallback(int arg)
            {
                Interlocked.Increment(ref callbackCount);
                tcs.SetResult(arg);
            }

            using var sut = new AsyncValueCalculator<int>(dispatcher, CalculationTaskFactory, HandleResultCallback);

            var option = Substitute.For<IValueOption>();
            sut.Affect(option);

            option.When(o => o.IsLoading = false)
                .Do(x => updateFinishBlock.Set());
            
            // Act
            sut.Update();
            updateStartBlock.Wait();
            sut.Update();

            // Assert
            await tcs.Task;
            updateFinishBlock.Wait(TimeSpan.FromSeconds(1));

            Assert.Equal(1, callbackCount);
            Assert.Equal(2, calculationCount);

            Assert.False(option.IsLoading);

            option.Received(2).IsLoading = true;
            option.Received(1).IsLoading = false;
        }

        [Fact]
        public async Task UpdateShouldNotThrowWhenTaskThrows()
        {
            // Arrange
            var tcs = new TaskCompletionSource<int>();
            var dispatcher = CreateDispatcher();

            Task<IValueCalculationResult<int>> CalculationTaskFactory(CancellationToken ct)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(15);
                    tcs.SetResult(0);
                });

                return Task.FromException<IValueCalculationResult<int>>(new Exception("Test"));
            }

            void HandleResultCallback(int arg)
            {
                tcs.SetResult(arg);
            }

            using var sut = new AsyncValueCalculator<int>(dispatcher, CalculationTaskFactory, HandleResultCallback);

            var option = Substitute.For<IValueOption>();
            sut.Affect(option);

            // Act
            sut.Update();

            // Assert
            var result = await tcs.Task;

            option.Received(1).IsLoading = true;
            option.Received(2).IsLoading = false;

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task UpdateShouldSetAndResetLoadingFlagOfAffectedOptions()
        {
            // Arrange
            var tcs = new TaskCompletionSource<int>();
            var dispatcher = CreateDispatcher();

            async Task<IValueCalculationResult<int>> CalculationTaskFactory(CancellationToken ct)
            {
                await Task.Delay(50, ct);
                return ValueCalculationResult.Success(123);
            }

            void HandleResultCallback(int arg)
            {
                tcs.SetResult(arg);
            }

            using var sut = new AsyncValueCalculator<int>(dispatcher, CalculationTaskFactory, HandleResultCallback);

            var option = Substitute.For<IValueOption>();
            sut.Affect(option);

            // Act
            sut.Update();

            // Assert
            await tcs.Task;

            Assert.False(option.IsLoading);

            option.Received(1).IsLoading = true;
            option.Received(1).IsLoading = false;
        }
    }
}