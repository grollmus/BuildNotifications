using System;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.PluginInterfaces.Host;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options
{
    public class AsyncValueCalculatorTests
    {
        public AsyncValueCalculatorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

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
            // Arrange
            const int runs = 3;

            var tcs = new TaskCompletionSource<int>();
            var dispatcher = CreateDispatcher();
            var callbackCount = 0;
            var calculationCount = 0;

            Task<IValueCalculationResult<int>> CalculationTaskFactory(CancellationToken ct)
            {
                _output.WriteLine($"{DateTime.Now.Ticks} - Calc {calculationCount}");
                ++calculationCount;
                Thread.Sleep(100);
                return Task.FromResult<IValueCalculationResult<int>>(ValueCalculationResult.Success(123));
            }

            void HandleResultCallback(int arg)
            {
                _output.WriteLine($"{DateTime.Now.Ticks} - Callback {callbackCount}");
                ++callbackCount;
                tcs.SetResult(arg);
            }

            using var sut = new AsyncValueCalculator<int>(dispatcher, CalculationTaskFactory, HandleResultCallback);

            var option = Substitute.For<IValueOption>();
            sut.Affect(option);

            // Act
            for (var i = 0; i < runs; ++i)
            {
                _output.WriteLine($"{DateTime.Now.Ticks} - Update {i}");
                sut.Update();
                Thread.Sleep(5);
            }

            // Assert
            await tcs.Task;

            Assert.Equal(1, callbackCount);
            Assert.Equal(runs, calculationCount);

            option.Received(runs).IsLoading = true;
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
            option.Received(1).IsLoading = false;

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task UpdateShouldSetAndResetLoadingFlagOfAffectedOptions()
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
            sut.Affect(option);

            // Act
            sut.Update();

            // Assert
            await tcs.Task;

            option.Received(1).IsLoading = true;
            option.Received(1).IsLoading = false;
        }
    }
}