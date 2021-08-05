using Microsoft.Extensions.DependencyInjection;
using TrueLayerChallenge.Domain.Database;

namespace TrueLayerChallenge.DataStore.InMemoryCache
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataStore(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPokemonDatastore, InMemoryCache>();
        }
    }
}
