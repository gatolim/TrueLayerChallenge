using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Database;
using TrueLayerChallenge.Domain.Queries;
using TrueLayerChallenge.Domain.QueryHandlers;
using TrueLayerChallenge.Domain.QueryModels;
using TrueLayerChallenge.Domain.Services;
using Xunit;

namespace TrueLayerChallenge.Domain.Test
{
    public class GetPokemonDetailsTest
    {
        private Mock<IPokemonDatastore> _pokemonStoreMock = new Mock<IPokemonDatastore>();
        private Mock<IPokemonService> _pokemonServiceMock = new Mock<IPokemonService>();
        private Mock<ILogger<GetPokemonHandler>> _loggerMock = new Mock<ILogger<GetPokemonHandler>>();
        private GetPokemonHandler _queryHandler;

        [Fact]
        public async Task GetPokemonExistedInCache()
        {
            var query = new GetPokemonDetails()
            {
                PokemonName = "PokemonFromCache"
            };

            _pokemonStoreMock
               .Setup(p => p.GetPokemonAsync(query.PokemonName))
               .Returns(Task.FromResult(new Pokemon() { Name = query.PokemonName }));

            _queryHandler = new GetPokemonHandler(_pokemonStoreMock.Object, _pokemonServiceMock.Object, _loggerMock.Object);
            Pokemon pokemon = await _queryHandler.ReadAsync(query);


            Assert.Equal(query.PokemonName, pokemon?.Name);
            _pokemonServiceMock.Verify(s => s.GetPokemonDetailsAsync(query.PokemonName), Times.Never);
            _pokemonStoreMock.Verify(s => s.WritePokemonAsync(It.IsAny<Pokemon>()), Times.Never);

        }

        [Fact]
        public async Task GetPokemonNoInCache()
        {
            var query = new GetPokemonDetails()
            {
                PokemonName = "PokemonNotFromCache"
            };

            _pokemonStoreMock
               .Setup(p => p.GetPokemonAsync(query.PokemonName))
               .Returns(Task.FromResult<Pokemon>(null));

            _pokemonServiceMock
               .Setup(p => p.GetPokemonDetailsAsync(query.PokemonName))
               .Returns(Task.FromResult(new Pokemon() { Name = query.PokemonName }));


            _queryHandler = new GetPokemonHandler(_pokemonStoreMock.Object, _pokemonServiceMock.Object, _loggerMock.Object);
            Pokemon pokemon = await _queryHandler.ReadAsync(query);


            Assert.Equal(query.PokemonName, pokemon?.Name);
            _pokemonServiceMock.Verify(s => s.GetPokemonDetailsAsync(query.PokemonName), Times.Once);
            _pokemonStoreMock.Verify(s => s.GetPokemonAsync(query.PokemonName), Times.Once);
            _pokemonStoreMock.Verify(s => s.WritePokemonAsync(It.IsAny<Pokemon>()), Times.Once);
        }

        [Fact]
        public async Task GetUnknowPokemon()
        {
            var query = new GetPokemonDetails()
            {
                PokemonName = "UnknownPokemon"
            };

            _pokemonStoreMock
               .Setup(p => p.GetPokemonAsync(query.PokemonName))
               .Returns(Task.FromResult<Pokemon>(null));

            _pokemonServiceMock
               .Setup(p => p.GetPokemonDetailsAsync(query.PokemonName))
               .Returns(Task.FromResult<Pokemon>(null));


            _queryHandler = new GetPokemonHandler(_pokemonStoreMock.Object, _pokemonServiceMock.Object, _loggerMock.Object);
            Pokemon pokemon = await _queryHandler.ReadAsync(query);


            Assert.True(pokemon == null);
            _pokemonServiceMock.Verify(s => s.GetPokemonDetailsAsync(query.PokemonName), Times.Once);
            _pokemonStoreMock.Verify(s => s.GetPokemonAsync(query.PokemonName), Times.Once);
            _pokemonStoreMock.Verify(s => s.WritePokemonAsync(It.IsAny<Pokemon>()), Times.Never);
        }
    }
}
