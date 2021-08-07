using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Database;
using TrueLayerChallenge.Domain.Exceptions;
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
            if (string.IsNullOrWhiteSpace(query.PokemonName))
                return null;

            // First check the inmemory cache see if pokemon existed, if so use that instead.
            Pokemon cachedPokemon = await _dataStore.GetPokemonAsync(query.PokemonName);
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
                    // either log a warning or
                    // log an error and throw custom exception with suggest solution and let API Global handler to handle those.
                }
            }

            return cachedPokemon;
        }
    }
}
