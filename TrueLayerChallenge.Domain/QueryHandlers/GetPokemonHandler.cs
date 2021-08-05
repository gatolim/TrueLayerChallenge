using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Database;
using TrueLayerChallenge.Domain.Queries;
using TrueLayerChallenge.Domain.QueryModels;
using TrueLayerChallenge.Domain.Services;

namespace TrueLayerChallenge.Domain.QueryHandlers
{
    public class GetPokemonHandler : IQueryHandler<GetPokemonDetails, Pokemon>
    {
        private IPokemonDatastore _dataStore;
        private IPokemonService _pokemonService;
        private ILogger _logger;

        public GetPokemonHandler(IPokemonDatastore dataStore, IPokemonService pokemonService, ILogger<GetPokemonHandler> logger)
        {
            _dataStore = dataStore;
            _pokemonService = pokemonService;
            _logger = logger;
        }

        public async Task<Pokemon> ReadAsync(GetPokemonDetails query)
        {
            Pokemon cachedPokemon = null;
            cachedPokemon = await _dataStore.GetPokemonAsync(query.PokemonName);
            if (cachedPokemon == null)
            {
                cachedPokemon = await _pokemonService.GetPokemonDetailsAsync(query.PokemonName);

                if (cachedPokemon != null) { await _dataStore.WritePokemonAsync(cachedPokemon); }
            }

            return cachedPokemon;
        }
    }
}
