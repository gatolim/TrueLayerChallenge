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

        public GetPokemonHandler(IPokemonDatastore dataStore, IPokemonService pokemonService)
        {
            _dataStore = dataStore;
            _pokemonService = pokemonService;
        }

        public async Task<Pokemon> ReadAsync(GetPokemonDetails query)
        {
            Pokemon cachedPokemon = null;
            cachedPokemon = await _dataStore.GetPokemonAsync(query.PokemonName);
            if (cachedPokemon == null)
            {
                var response = await _pokemonService.GetPokemonDetailsAsync(query.PokemonName);

                if (response.IsSucceed)
                {
                    cachedPokemon = response.Data;
                    await _dataStore.WritePokemonAsync(cachedPokemon);
                }
                else
                {
                    // throw custom exception with suggest solution and let API Global handler to handle those.
                }
            }

            return cachedPokemon;
        }
    }
}
