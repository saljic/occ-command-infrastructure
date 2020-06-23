using System.Threading;
using System.Threading.Tasks;

namespace Command.Infrastructure.Api.Commands
{
    /// <summary>
    ///     The <see cref="ICommandBus"/> is used to send <see cref="ICommand"/>'s trough.
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        ///     Returns a <see cref="Task"/> that represents the asynchronous processing of the provided <paramref name="command"/>.
        /// </summary>
        Task SendAsync(ICommand command);

        /// <summary>
        ///     Returns a <see cref="Task"/> that represents the asynchronous processing of the provided <paramref name="command"/>, with cancellation support.
        /// </summary>
        Task SendAsync(ICommand command, CancellationToken token);

        /// <summary>
        ///     Returns a <see cref="Task{TResult}"/> that represents the asynchronous processing of the provided <paramref name="command"/>.
        ///     The task result contains the <typeparamref name="TResult"/>.
        /// </summary>
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command);

        /// <summary>
        ///     Returns a <see cref="Task{TResult}"/> that represents the asynchronous processing of the provided <paramref name="command"/>, with cancellation support.
        ///     The task result contains the <typeparamref name="TResult"/>.
        /// </summary>
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken token);
    }
}