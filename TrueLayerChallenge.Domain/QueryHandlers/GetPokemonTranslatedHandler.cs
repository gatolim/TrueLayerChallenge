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
            var cachedPokemon = await _dataStore.GetPokemonAsync(query.PokemonName);
            if (cachedPokemon == null)
            {
                cachedPokemon = await _pokemonService.GetPokemonDetailsAsync(query.PokemonName);
            }

            if (cachedPokemon != null && string.IsNullOrWhiteSpace(cachedPokemon.TranslatedDescription))
            {
                if ((!string.IsNullOrWhiteSpace(cachedPokemon.Habitat) && cachedPokemon.Habitat.Equals("cave", StringComparison.OrdinalIgnoreCase)) || cachedPokemon.IsLegendary)
                {
                    cachedPokemon.TranslatedDescription = await _translateService.GetYodaTranslationAsync(cachedPokemon.StandardDescription);
                }
                else
                {
                    cachedPokemon.TranslatedDescription = await _translateService.GetShakespeareTranslationAsync(cachedPokemon.StandardDescription);
                }
            }

            if (cachedPokemon != null)
                await _dataStore.WritePokemonAsync(cachedPokemon);

            return cachedPokemon;
        }
    }
}
