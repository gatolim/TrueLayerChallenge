using Moq;
using System;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain;
using TrueLayerChallenge.Domain.Queries;
using TrueLayerChallenge.Domain.QueryHandlers;
using TrueLayerChallenge.WebApi.Controllers;
using TrueLayerChallenge.WebApi.Models;
using Xunit;
using QueryModel = TrueLayerChallenge.Domain.QueryModels;

namespace TrueLayerChallenge.WebApi.Test
{
    public class PokemonControllerTest
    {
        private Mock<IQueryProcessor> _queryProcessor = new Mock<IQueryProcessor>();
        private Mock<IQueryHandler<GetPokemonDetails, QueryModel.Pokemon>> _pokemonHandler = new Mock<IQueryHandler<GetPokemonDetails, QueryModel.Pokemon>>();
        private Mock<IQueryHandler<GetPokemonTranslatedDetails, QueryModel.Pokemon>> _pokemonTranslatedHandler = new Mock<IQueryHandler<GetPokemonTranslatedDetails, QueryModel.Pokemon>>();
        private PokemonController _controller;

        [Fact]
        public async Task GetPokemon()
        {
            var pokemonName = "TestPokemon";
            
            _queryProcessor
               .Setup(p => p.ProcessQueryAsync<IQuery, QueryModel.Pokemon>(It.IsAny<IQuery>()))
               .Returns(Task.FromResult(new QueryModel.Pokemon() { 
                   Name = pokemonName, 
                   StandardDescription="StandardText", 
                   TranslatedDescription="TranslatedText" }));

            _controller = new PokemonController(_queryProcessor.Object);
            var response = await _controller.GetPokemon_V1(pokemonName);

            Assert.NotNull(response);
            var result = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(response);
            var pokemon = Assert.IsType<Pokemon>(result.Value);
            Assert.NotNull(pokemon);
            Assert.Equal(pokemon.Name, pokemonName);
            Assert.Equal("StandardText", pokemon.Description);
        }

        [Fact]
        public async Task GetPokemonNoResult()
        {
            var pokemonName = "TestPokemon";

            _queryProcessor
               .Setup(p => p.ProcessQueryAsync<IQuery, QueryModel.Pokemon>(It.IsAny<IQuery>()))
               .Returns(Task.FromResult<QueryModel.Pokemon>(null));

            _controller = new PokemonController(_queryProcessor.Object);
            var response = await _controller.GetPokemon_V1(pokemonName);

            Assert.NotNull(response);
            var result = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(response);
        }


        [Fact]
        public async Task GetTranslatedPokemon()
        {
            var pokemonName = "TestPokemon";

            _queryProcessor
               .Setup(p => p.ProcessQueryAsync<IQuery, QueryModel.Pokemon>(It.IsAny<IQuery>()))
               .Returns(Task.FromResult(new QueryModel.Pokemon()
               {
                   Name = pokemonName,
                   StandardDescription = "StandardText",
                   TranslatedDescription = "TranslatedText"
               }));

            _controller = new PokemonController(_queryProcessor.Object);
            var response = await _controller.GetTranslatedPokemon_V1(pokemonName);

            Assert.NotNull(response);
            var result = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(response);
            var pokemon = Assert.IsType<Pokemon>(result.Value);
            Assert.NotNull(pokemon);
            Assert.Equal(pokemon.Name, pokemonName);
            Assert.Equal("TranslatedText", pokemon.Description);
        }

        [Fact]
        public async Task GetTranslatedPokemonNoResult()
        {
            var pokemonName = "TestPokemon";

            _queryProcessor
               .Setup(p => p.ProcessQueryAsync<IQuery, QueryModel.Pokemon>(It.IsAny<IQuery>()))
               .Returns(Task.FromResult<QueryModel.Pokemon>(null));

            _controller = new PokemonController(_queryProcessor.Object);
            var response = await _controller.GetTranslatedPokemon_V1(pokemonName);

            Assert.NotNull(response);
            var result = Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(response);
        }
    }
}
