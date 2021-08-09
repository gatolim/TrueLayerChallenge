using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using TrueLayerChallenge.Domain.QueryModels;
using TrueLayerChallenge.Domain.Services;
using Xunit;

namespace TrueLayerChallenge.Service.PokemonService.Test
{
    public class PokemonServiceTest
    {
        private MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
        private HttpClient _httpClient;
        private Mock<ILogger<PokemonService>> logger = new Mock<ILogger<PokemonService>>();
        private PokemonService _service;

        [Theory]
        [InlineData("AllValuePresent", "rare", "testing 1", "en", true)]
        [InlineData("MissingLegendary", "rare", "testing 1", "en", null)]
        [InlineData("MissingText", "rare", "", "en", true)]
        [InlineData("NoEnglishText", "rare", "testing 1", "eu", true)]
        [InlineData("MissingHabitat", "", "testing 1", "en", true)]
        public async void TestNullValueFromApi(string pokemonName, string habitat, string falavorText, string culture, bool? isLegendary)
        {
            // Mock test data 
            dynamic dynamicPokemon = new JObject() as dynamic;
            dynamicPokemon.name = pokemonName;
            dynamicPokemon.is_legendary = isLegendary;
            dynamicPokemon.habitat = new JObject() as dynamic;
            dynamicPokemon.habitat.name = habitat;

            dynamicPokemon.flavor_text_entries = new JArray() as dynamic;

            var flavorTextArray = new JObject() as dynamic;
            flavorTextArray.flavor_text = falavorText;
            flavorTextArray.language = new JObject() as dynamic;
            flavorTextArray.language.name = culture;

            dynamicPokemon.flavor_text_entries.Add(flavorTextArray);

            string mockPokemon = dynamicPokemon.ToString();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .Respond("application/json", mockPokemon); // Respond with JSON


            _httpClient = new HttpClient(mockHttp);
            _service = new PokemonService(_httpClient, logger.Object);
            HttpResultResponse<Pokemon> response = await _service.GetPokemonDetailsAsync(pokemonName);

            Assert.True(response.IsSucceed);

            Pokemon pokemon = response.Data;

            Assert.NotNull(pokemon);
            Assert.Equal(pokemonName, pokemon.Name);
            Assert.Equal(habitat, pokemon.Habitat);
            Assert.Equal(isLegendary.HasValue ? isLegendary.Value : false, pokemon.IsLegendary);
            if (culture == "en")
                Assert.Equal(falavorText, pokemon.StandardDescription);
            else
                Assert.True(string.IsNullOrWhiteSpace(pokemon.StandardDescription));
        }

        [Fact]
        public async void TestInvalidResponseStructureFromApi()
        {
            // Mock test data 
            string pokemonName = "testPokemon";
            dynamic dynamicPokemon = new JObject() as dynamic;
            dynamicPokemon.name = pokemonName;
            dynamicPokemon.is_legendary = false;
           
            dynamicPokemon.flavor_text_entries = new JArray() as dynamic;

            string mockPokemon = dynamicPokemon.ToString();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .Respond("application/json", mockPokemon); // Respond with JSON


            _httpClient = new HttpClient(mockHttp);
            _service = new PokemonService(_httpClient, logger.Object);
            HttpResultResponse<Pokemon> response = await _service.GetPokemonDetailsAsync(pokemonName);

            Assert.True(response.IsSucceed);

            Pokemon pokemon = response.Data;

            Assert.NotNull(pokemon);
            Assert.Equal(pokemonName, pokemon.Name);
            Assert.True(string.IsNullOrWhiteSpace(pokemon.Habitat));
            Assert.False(pokemon.IsLegendary);
            Assert.True(string.IsNullOrWhiteSpace(pokemon.StandardDescription));
        }

        [Fact]
        public async void TestNoResponseFromApi()
        {
            var pokemonName = "test";
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .Respond("application/json", ""); // Respond with JSON


            _httpClient = new HttpClient(mockHttp);
            _service = new PokemonService(_httpClient, logger.Object);
            HttpResultResponse<Pokemon> response = await _service.GetPokemonDetailsAsync(pokemonName);

            Assert.False(response.IsSucceed);
            Assert.Null(response.Data);
            Assert.Equal($"No result were found for {pokemonName}", response.ErrorMessage);
        }

        [Fact]
        public async void TestErrorFromApi()
        {
            var pokemonName = "test";
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
                .Throw(new HttpRequestException());

            _httpClient = new HttpClient(mockHttp);
            _service = new PokemonService(_httpClient, logger.Object);
            HttpResultResponse<Pokemon> response = await _service.GetPokemonDetailsAsync(pokemonName);

            Assert.False(response.IsSucceed);
            Assert.Null(response.Data);
        }
    }
}
