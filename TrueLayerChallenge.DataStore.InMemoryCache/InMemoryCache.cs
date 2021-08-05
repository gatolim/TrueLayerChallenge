using System.Collections.Concurrent;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Database;
using TrueLayerChallenge.Domain.QueryModels;

namespace TrueLayerChallenge.DataStore.InMemoryCache
{
    public class InMemoryCache : IPokemonDatastore
    {
        private ConcurrentDictionary<string, Pokemon> _dataStore;

        public InMemoryCache()
        {
            _dataStore = new ConcurrentDictionary<string, Pokemon>();
        }

        public Task<Pokemon> GetPokemonAsync(string name)
        {
            Pokemon cachedPokemon = null;
            _dataStore.TryGetValue(name, out cachedPokemon);
            return Task.FromResult(cachedPokemon);
        }

        public Task WritePokemonAsync(Pokemon pokemon)
        {
            _dataStore[pokemon.Name] = pokemon;
            return Task.CompletedTask;
        }
    }
}
