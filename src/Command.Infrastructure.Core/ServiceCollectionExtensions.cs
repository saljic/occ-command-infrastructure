using Command.Infrastructure.Api.Commands;
using Command.Infrastructure.Core.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Command.Infrastructure.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandInfrastructure(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICommandBus, CommandBus>();
            // Maybe add support for scoped resolving using config object, so that scoped services injected in commands get disposed automatically.
            serviceCollection.AddTransient(typeof(CommandBus.CommandHandlerProxy<,>));
            serviceCollection.AddTransient(typeof(CommandBus.CommandHandlerGenericProxy<,,>));
            return serviceCollection;
        }
    }
}