using System.Threading;
using System.Threading.Tasks;

namespace Command.Infrastructure.Api.Commands
{
    /// <summary>
    ///     The <see cref="ICommandHandler{TCommand}"/> is used to implement the logic of the <typeparamref name="TCommand"/>.
    /// </summary>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        ///     Returns a <see cref="Task"/> which represents the asynchronous operation of processing the <paramref name="command"/>, with cancellation support.
        /// </summary>
        Task Handle(TCommand command, CancellationToken token);
    }

    /// <summary>
    ///     The <see cref="ICommandHandler{TCommand, TResult}"/> is used to implement the logic of the <typeparamref name="TCommand"/>.
    /// </summary>
    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        ///     Returns a task which represents the asynchronous operation of processing the <paramref name="command"/>, with cancellation support.
        /// </summary>
        Task<TResult> Handle(TCommand command, CancellationToken token);
    }
}