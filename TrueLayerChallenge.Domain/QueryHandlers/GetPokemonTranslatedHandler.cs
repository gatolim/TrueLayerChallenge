using System;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Database;
using TrueLayerChallenge.Domain.Queries;
using TrueLayerChallenge.Domain.QueryModels;
using TrueLayerChallenge.Domain.Services;

namespace TrueLayerChallenge.Domain.QueryHandlers
{
    public class GetPokemonTranslatedHandler : IQueryHandler<GetPokemonTranslatedDetails, Pokemon>
    {
        private IPokemonDatastore _dataStore;
        private IPokemonService _pokemonService;
        private ITranslationService _translateService;

        public GetPokemonTranslatedHandler(IPokemonDatastore dataStore, IPokemonService pokemonService, ITranslationService translateService)
        {
            _dataStore = dataStore;
            _pokemonService = pokemonService;
            _translateService = translateService;
        }

        public async Task<Pokemon> ReadAsync(GetPokemonTranslatedDetails query)
        {
            // First check the inmemory cache see if pokemon existed, if so use that instead.
            var cachedPokemon = await _dataStore.GetPokemonAsync(query.PokemonName);
            if (cachedPokemon == null)
            {
                var response = await _pokemonService.GetPokemonDetailsAsync(query.PokemonName);
                if (response.IsSucceed)
                {
                    cachedPokemon = response.Data;
                }
                else
                {
                    // throw custom exception with suggest solution and let API Global handler to handle those.
                }
            }

            // Attempt to translate the pokemon if it hasn't been already translated.
            if (cachedPokemon != null && string.IsNullOrWhiteSpace(cachedPokemon.TranslatedDescription))
            {
                HttpResultResponse<string> response;
                if (ShouldUseYoda(cachedPokemon))
                {
                    response = await _translateService.GetYodaTranslationAsync(cachedPokemon.StandardDescription);
                }
                else
                {
                    response = await _translateService.GetShakespeareTranslationAsync(cachedPokemon.StandardDescription);
                }

                if (response.IsSucceed)
                {
                    cachedPokemon.TranslatedDescription = response.Data;
                }
                else
                {
                    // throw custom exception with suggest solution and let API Global handler to handle those.
                }
            }

            // update cache with latest info
            if (cachedPokemon != null)
                await _dataStore.WritePokemonAsync(cachedPokemon);

            return cachedPokemon;
        }

        protected bool ShouldUseYoda(Pokemon pokemon)
        {
            return (!string.IsNullOrWhiteSpace(pokemon.Habitat) && pokemon.Habitat.Equals("cave", StringComparison.OrdinalIgnoreCase)) || pokemon.IsLegendary;
        }
    }
}
