using Microsoft.Extensions.DependencyInjection;
using TrueLayerChallenge.Domain.Services;

namespace TrueLayerChallenge.Service.PokemonService
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPokemonService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient<IPokemonService, PokemonService>();
        }
    }
}
