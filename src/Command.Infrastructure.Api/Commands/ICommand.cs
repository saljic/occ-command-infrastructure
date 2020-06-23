namespace Command.Infrastructure.Api.Commands
{
    /// <summary>
    ///     The <see cref="ICommand"/> is used to define commands with no return value.
    /// </summary>
    public interface ICommand
    {
    }

    /// <summary>
    ///     The <see cref="ICommand{TResult}"/> is used to define commands that have a <typeparamref name="TResult"/>.
    /// </summary>
    public interface ICommand<TResult> : ICommand
    {
    }
}