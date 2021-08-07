using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.QueryModels;
using TrueLayerChallenge.Domain.Services;

namespace TrueLayerChallenge.Service.PokemonService
{
    /// <summary>
    /// A simple service that wrapped with an HttpClient, which in used to interact with external endpoint.
    /// </summary>
    public class PokemonService : IPokemonService
    {
        public HttpClient Client { get; }
        private ILogger _logger;

        public PokemonService(HttpClient client, ILogger<PokemonService> logger)
        {
            Client = client;
            Client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
            _logger = logger;
        }

        public async Task<HttpResultResponse<Pokemon>> GetPokemonDetailsAsync(string pokemonName)
        {
            try
            {
                var jsonResponse = await Client.GetStringAsync($"pokemon-species/{pokemonName}");

                if (string.IsNullOrWhiteSpace(jsonResponse))
                {
                    return HttpResultResponse<Pokemon>.Error($"No result were found for {pokemonName}");
                }

                // Prase result into dynamic so we don't need to maintain a model. 
                // This should be sufficient for this purpose of this exercise.
                dynamic json = JValue.Parse(jsonResponse);

                string name = json.name;

                if (name != pokemonName)
                {
                    // the result is not what I wanted.
                    var msg = $"No result found for Pokemon - {pokemonName}, found {name} instead";
                    _logger.LogWarning(msg);
                    return HttpResultResponse<Pokemon>.Error(msg);
                }

                bool? isLegendary = json.is_legendary;
                string habitat = json.habitat.name;
                string description = string.Empty;

                foreach (dynamic flavor_text in json.flavor_text_entries)
                {
                    if (flavor_text.language.name == "en")
                    {
                        description = Regex.Replace((string)flavor_text.flavor_text, @"\t|\n|\r|\f", " ");
                        break;
                    }
                }

                return HttpResultResponse<Pokemon>.OK(new Pokemon()
                {
                    Name = name,
                    IsLegendary = isLegendary.HasValue ? isLegendary.Value : false,
                    Habitat = habitat,
                    StandardDescription = description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed Getting Pokemon - {pokemonName}, Error: {ex.ToString()}");
                return HttpResultResponse<Pokemon>.Error($"{ex.Message}");
            }
        }
    }
}
