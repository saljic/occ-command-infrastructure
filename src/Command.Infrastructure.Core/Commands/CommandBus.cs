using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Command.Infrastructure.Api.Commands;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Command.Infrastructure.UnitTest")]

namespace Command.Infrastructure.Core.Commands
{
    internal sealed class CommandBus : ICommandBus
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SendAsync(ICommand command) => await SendAsync(command, CancellationToken.None);

        public async Task SendAsync(ICommand command, CancellationToken token)
        {
            var commandType = command.GetType();
            var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var commandHandlerProxyType =
                typeof(CommandHandlerProxy<,>).MakeGenericType(commandHandlerType, commandType);

            var commandHandlerProxy =
                (ICommandHandlerProxy) _serviceProvider.GetRequiredService(commandHandlerProxyType);
            await commandHandlerProxy.Handle(command, token);
        }

        public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command) =>
            await SendAsync(command, CancellationToken.None);

        public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken token)
        {
            var commandType = command.GetType();
            var commandHandlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
            var commandHandlerProxyType =
                typeof(CommandHandlerGenericProxy<,,>).MakeGenericType(commandHandlerType, commandType,
                    typeof(TResult));

            var commandHandlerProxy =
                (ICommandHandlerGenericProxy<TResult>) _serviceProvider.GetRequiredService(commandHandlerProxyType);
            var result = await commandHandlerProxy.Handle(command, token);
            return result;
        }

        internal interface ICommandHandlerGenericProxy<TResult>
        {
            Task<TResult> Handle(ICommand command, CancellationToken token);
        }

        internal sealed class
            CommandHandlerGenericProxy<TCommandHandler, TCommand, TResult> : ICommandHandlerGenericProxy<TResult>
            where TCommandHandler : ICommandHandler<TCommand, TResult>
            where TCommand : ICommand<TResult>
        {
            private readonly TCommandHandler _commandHandler;

            public CommandHandlerGenericProxy(TCommandHandler commandHandler)
            {
                _commandHandler = commandHandler;
            }

            public async Task<TResult> Handle(ICommand command, CancellationToken token) =>
                await _commandHandler.Handle((TCommand) command, token);
        }

        internal interface ICommandHandlerProxy
        {
            Task Handle(ICommand command, CancellationToken token);
        }

        internal sealed class CommandHandlerProxy<TCommandHandler, TCommand> : ICommandHandlerProxy
            where TCommandHandler : ICommandHandler<TCommand> where TCommand : ICommand
        {
            private readonly TCommandHandler _commandHandler;

            public CommandHandlerProxy(TCommandHandler commandHandler)
            {
                _commandHandler = commandHandler;
            }

            public async Task Handle(ICommand command, CancellationToken token) =>
                await _commandHandler.Handle((TCommand) command, token);
        }
    }
}