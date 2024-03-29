using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Database;
using TrueLayerChallenge.Domain.Enum;
using TrueLayerChallenge.Domain.Queries;
using TrueLayerChallenge.Domain.QueryHandlers;
using TrueLayerChallenge.Domain.QueryModels;
using TrueLayerChallenge.Domain.Services;
using Xunit;

namespace TrueLayerChallenge.Domain.Test
{
    public class GetPokemonTranslatedDetailsHandlerTest
    {
        private Mock<IPokemonDatastore> _pokemonStoreMock = new Mock<IPokemonDatastore>();
        private Mock<IPokemonService> _pokemonServiceMock = new Mock<IPokemonService>();
        private Mock<ITranslationService> _translationServiceMock = new Mock<ITranslationService>();
        private Mock<ILogger<GetPokemonHandler>> _loggerMock = new Mock<ILogger<GetPokemonHandler>>();
        private GetPokemonTranslatedHandler _queryHandler;

        [Fact]
        public async Task GetPokemonExistedInCache()
        {
            var query = new GetPokemonTranslatedDetails()
            {
                PokemonName = "PokemonFromCache"
            };

            _pokemonStoreMock
               .Setup(p => p.GetPokemonAsync(query.PokemonName))
               .Returns(Task.FromResult(new Pokemon() { Name = query.PokemonName, IsLegendary = true }));

            _translationServiceMock
                .Setup(p => p.GetTranslationAsync(It.IsAny<string>(), It.IsAny<TranslationType>()))
                .Returns(Task.FromResult(HttpResultResponse<string>.OK("YodaTranslation")));

            _queryHandler = new GetPokemonTranslatedHandler(_pokemonStoreMock.Object, _pokemonServiceMock.Object, _translationServiceMock.Object);
            Pokemon pokemon = await _queryHandler.ReadAsync(query);


            Assert.NotNull(pokemon);
            Assert.Equal(query.PokemonName, pokemon.Name);
            Assert.Equal("YodaTranslation", pokemon.TranslatedDescription);
            _pokemonServiceMock.Verify(s => s.GetPokemonDetailsAsync(It.IsAny<string>()), Times.Never);
            _translationServiceMock.Verify(s => s.GetTranslationAsync(It.IsAny<string>(), It.IsAny<TranslationType>()), Times.Once);
            _pokemonStoreMock.Verify(s => s.WritePokemonAsync(It.IsAny<Pokemon>()), Times.Once);

        }

        [Fact]
        public async Task GetPokemonNoInCache()
        {
            var query = new GetPokemonTranslatedDetails()
            {
                PokemonName = "PokemonNotFromCache"
            };

            _pokemonStoreMock
               .Setup(p => p.GetPokemonAsync(query.PokemonName))
               .Returns(Task.FromResult<Pokemon>(null));

            _pokemonServiceMock
               .Setup(p => p.GetPokemonDetailsAsync(query.PokemonName))
               .Returns(Task.FromResult(HttpResultResponse<Pokemon>.OK(new Pokemon() { Name = query.PokemonName, IsLegendary = true })));
            
            _translationServiceMock
                .Setup(p => p.GetTranslationAsync(It.IsAny<string>(), It.IsAny<TranslationType>()))
                .Returns(Task.FromResult(HttpResultResponse<string>.OK("YodaTranslation")));

            _queryHandler = new GetPokemonTranslatedHandler(_pokemonStoreMock.Object, _pokemonServiceMock.Object, _translationServiceMock.Object);
            Pokemon pokemon = await _queryHandler.ReadAsync(query);


            Assert.NotNull(pokemon);
            Assert.Equal(query.PokemonName, pokemon.Name);
            Assert.Equal("YodaTranslation", pokemon.TranslatedDescription);
            _pokemonServiceMock.Verify(s => s.GetPokemonDetailsAsync(query.PokemonName), Times.Once);
            _translationServiceMock.Verify(s => s.GetTranslationAsync(It.IsAny<string>(), It.IsAny<TranslationType>()), Times.Once);
            _pokemonStoreMock.Verify(s => s.GetPokemonAsync(query.PokemonName), Times.Once);
            _pokemonStoreMock.Verify(s => s.WritePokemonAsync(It.IsAny<Pokemon>()), Times.Once);
        }

        [Fact]
        public async Task GetUnknowPokemon()
        {
            var query = new GetPokemonTranslatedDetails()
            {
                PokemonName = "UnknownPokemon"
            };

            _pokemonStoreMock
               .Setup(p => p.GetPokemonAsync(query.PokemonName))
               .Returns(Task.FromResult<Pokemon>(null));

            _pokemonServiceMock
               .Setup(p => p.GetPokemonDetailsAsync(query.PokemonName))
               .Returns(Task.FromResult(HttpResultResponse<Pokemon>.Error($"No result were found for {query.PokemonName}")));


            _queryHandler = new GetPokemonTranslatedHandler(_pokemonStoreMock.Object, _pokemonServiceMock.Object, _translationServiceMock.Object);
            Pokemon pokemon = await _queryHandler.ReadAsync(query);


            Assert.Null(pokemon);
            _pokemonServiceMock.Verify(s => s.GetPokemonDetailsAsync(query.PokemonName), Times.Once);
            _translationServiceMock.Verify(s => s.GetTranslationAsync(It.IsAny<string>(), It.IsAny<TranslationType>()), Times.Never);
            _pokemonStoreMock.Verify(s => s.GetPokemonAsync(query.PokemonName), Times.Once);
            _pokemonStoreMock.Verify(s => s.WritePokemonAsync(It.IsAny<Pokemon>()), Times.Never);
        }

        [Fact]
        public async Task NotTranslateionFound()
        {
            var query = new GetPokemonTranslatedDetails()
            {
                PokemonName = "PokemonWithNoTranslation"
            };

            _pokemonStoreMock
               .Setup(p => p.GetPokemonAsync(query.PokemonName))
               .Returns(Task.FromResult(new Pokemon() { Name = query.PokemonName, IsLegendary = false, StandardDescription = "StandardText" }));

            _translationServiceMock
                .Setup(p => p.GetTranslationAsync(It.IsAny<string>(), It.IsAny<TranslationType>()))
                .Returns(Task.FromResult(HttpResultResponse<string>.Error("")));


            _queryHandler = new GetPokemonTranslatedHandler(_pokemonStoreMock.Object, _pokemonServiceMock.Object, _translationServiceMock.Object);
            Pokemon pokemon = await _queryHandler.ReadAsync(query);

            Assert.NotNull(pokemon);
            Assert.Equal(query.PokemonName, pokemon.Name);
            Assert.Equal("StandardText", pokemon.StandardDescription);
            Assert.True(string.IsNullOrWhiteSpace(pokemon.TranslatedDescription));
            _pokemonServiceMock.Verify(s => s.GetPokemonDetailsAsync(query.PokemonName), Times.Never);
            _translationServiceMock.Verify(s => s.GetTranslationAsync(It.IsAny<string>(), It.IsAny<TranslationType>()), Times.Once);
            _pokemonStoreMock.Verify(s => s.GetPokemonAsync(query.PokemonName), Times.Once);
            _pokemonStoreMock.Verify(s => s.WritePokemonAsync(It.IsAny<Pokemon>()), Times.Once);
        }
    }
}
