using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Command.Infrastructure.Api.Commands;
using Command.Infrastructure.Core.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Command.Infrastructure.UnitTest.Commands
{
    [TestClass]
    public class CommandBusTests
    {
        [TestMethod]
        public async Task CommandBus_SendAsync_ShouldResolveProxyTypeT_WhenSendAsyncT()
        {
            using var fakeCommandHandlerProxy = new FakeCommandHandlerProxy();
            var serviceProviderMock = new Mock<IServiceProvider>();

            var expectedType = typeof(CommandBus.CommandHandlerProxy<ICommandHandler<FakeCommand>, FakeCommand>);
            serviceProviderMock.Setup(x => x.GetService(expectedType)).Returns(fakeCommandHandlerProxy);

            var commandBus = new CommandBus(serviceProviderMock.Object);
            var fakeCommand = new FakeCommand();
            await commandBus.SendAsync(fakeCommand);

            serviceProviderMock.Verify(x => x.GetService(expectedType), Times.Once);
        }

        [TestMethod]
        public async Task CommandBus_SendAsync_ShouldInvokeFakeHandlerProxyWithT_WhenSendAsyncT()
        {
            using var fakeCommandHandlerProxy = new FakeCommandHandlerProxy();
            var serviceProviderMock = new Mock<IServiceProvider>();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(2));
            var handleOccured = fakeCommandHandlerProxy.OnHandle.FirstAsync().RunAsync(timeoutTokenSource.Token);

            var expectedType = typeof(CommandBus.CommandHandlerProxy<ICommandHandler<FakeCommand>, FakeCommand>);
            serviceProviderMock.Setup(x => x.GetService(expectedType)).Returns(fakeCommandHandlerProxy);

            var commandBus = new CommandBus(serviceProviderMock.Object);
            var fakeCommand = new FakeCommand();
            await commandBus.SendAsync(fakeCommand);

            var command = await handleOccured;
            Assert.AreSame(fakeCommand, command);
        }

        [TestMethod]
        public async Task CommandBus_SendAsync_ShouldResolveGenericProxyTypeT_WhenSendAsyncGenericT()
        {
            var result = new object();
            using var fakeCommandHandlerWithResult = new FakeCommandWithResultHandlerProxy(result);
            var serviceProviderMock = new Mock<IServiceProvider>();

            var expectedType =
                typeof(CommandBus.CommandHandlerGenericProxy<ICommandHandler<FakeCommandWithResult, object>,
                    FakeCommandWithResult, object>);
            serviceProviderMock.Setup(x => x.GetService(expectedType)).Returns(fakeCommandHandlerWithResult);

            var commandBus = new CommandBus(serviceProviderMock.Object);
            var fakeCommandWithResult = new FakeCommandWithResult();
            await commandBus.SendAsync(fakeCommandWithResult);

            serviceProviderMock.Verify(x => x.GetService(expectedType), Times.Once);
        }

        [TestMethod]
        public async Task CommandBus_SendAsync_ShouldReturnResultT_WhenSendAsyncGenericT()
        {
            var result = new object();
            using var fakeCommandHandlerWithResult = new FakeCommandWithResultHandlerProxy(result);
            var serviceProviderMock = new Mock<IServiceProvider>();

            var expectedType =
                typeof(CommandBus.CommandHandlerGenericProxy<ICommandHandler<FakeCommandWithResult, object>,
                    FakeCommandWithResult, object>);
            serviceProviderMock.Setup(x => x.GetService(expectedType)).Returns(fakeCommandHandlerWithResult);

            var commandBus = new CommandBus(serviceProviderMock.Object);
            var fakeCommandWithResult = new FakeCommandWithResult();
            var commandResult = await commandBus.SendAsync(fakeCommandWithResult);

            Assert.AreSame(result, commandResult);
        }

        [TestMethod]
        public async Task CommandBus_SendAsync_ShouldInvokeFakeHandlerGenericProxyWithT_WhenSendAsyncGenericT()
        {
            var result = new object();
            using var fakeCommandHandlerWithResult = new FakeCommandWithResultHandlerProxy(result);
            var serviceProviderMock = new Mock<IServiceProvider>();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(2));
            var handleOccured = fakeCommandHandlerWithResult.OnHandle.FirstAsync().RunAsync(timeoutTokenSource.Token);

            var expectedType =
                typeof(CommandBus.CommandHandlerGenericProxy<ICommandHandler<FakeCommandWithResult, object>,
                    FakeCommandWithResult, object>);
            serviceProviderMock.Setup(x => x.GetService(expectedType)).Returns(fakeCommandHandlerWithResult);

            var commandBus = new CommandBus(serviceProviderMock.Object);
            var fakeCommandWIthResult = new FakeCommandWithResult();
            await commandBus.SendAsync(fakeCommandWIthResult);

            var command = await handleOccured;
            Assert.AreSame(fakeCommandWIthResult, command);
        }

        private sealed class FakeCommand : ICommand
        {
        }

        private sealed class FakeCommandHandlerProxy : CommandBus.ICommandHandlerProxy, IDisposable
        {
            private readonly Subject<ICommand> _commandSubject = new Subject<ICommand>();
            public IObservable<ICommand> OnHandle => _commandSubject.AsObservable();
            public void Dispose() => _commandSubject?.Dispose();

            public Task Handle(ICommand command, CancellationToken token)
            {
                _commandSubject.OnNext(command);
                return Task.CompletedTask;
            }
        }

        private sealed class FakeCommandWithResult : ICommand<object>
        {
        }

        private sealed class FakeCommandWithResultHandlerProxy : CommandBus.ICommandHandlerGenericProxy<object>,
            IDisposable
        {
            private readonly Subject<ICommand> _commandSubject = new Subject<ICommand>();
            private readonly object _result;

            public IObservable<ICommand> OnHandle => _commandSubject.AsObservable();
            public void Dispose() => _commandSubject?.Dispose();

            public FakeCommandWithResultHandlerProxy(object result)
            {
                _result = result;
            }

            public Task<object> Handle(ICommand command, CancellationToken token)
            {
                _commandSubject.OnNext(command);
                return Task.FromResult(_result);
            }
        }
    }
}