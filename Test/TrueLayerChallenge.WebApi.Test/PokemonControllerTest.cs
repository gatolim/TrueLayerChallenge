using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain;
using TrueLayerChallenge.Domain.Queries;
using TrueLayerChallenge.WebApi.Controllers;
using TrueLayerChallenge.WebApi.Middleware;
using TrueLayerChallenge.WebApi.Models;
using Xunit;
using QueryModel = TrueLayerChallenge.Domain.QueryModels;

namespace TrueLayerChallenge.WebApi.Test
{
    public class PokemonControllerTest
    {
        private Mock<IQueryProcessor> _queryProcessor = new Mock<IQueryProcessor>();
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
            Assert.Equal($"The requested pokemon - {pokemonName} is not found", result.Value);
        }


        [Theory]
        [InlineData("StandardText", "TranslatedText")]
        [InlineData("", "TranslatedText")]
        [InlineData("StandardText", "")]
        [InlineData("", "")]
        public async Task GetTranslatedPokemon(string standardText, string translatedText)
        {
            var pokemonName = "TestPokemon";

            _queryProcessor
               .Setup(p => p.ProcessQueryAsync<IQuery, QueryModel.Pokemon>(It.IsAny<IQuery>()))
               .Returns(Task.FromResult(new QueryModel.Pokemon()
               {
                   Name = pokemonName,
                   StandardDescription = standardText,
                   TranslatedDescription = translatedText
               }));

            _controller = new PokemonController(_queryProcessor.Object);
            var response = await _controller.GetTranslatedPokemon_V1(pokemonName);

            Assert.NotNull(response);
            var result = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(response);
            var pokemon = Assert.IsType<Pokemon>(result.Value);
            Assert.NotNull(pokemon);
            Assert.Equal(pokemon.Name, pokemonName);
            Assert.Equal(translatedText??standardText, pokemon.Description);
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
            Assert.Equal($"The requested pokemon - {pokemonName} is not found", result.Value);
        }

        [Fact]
        public async Task TestGlobalExceptionHanlder()
        {         
            var httpContextMoq = new Mock<HttpContext>();
            httpContextMoq.Setup(x => x.Response)
                .Returns(new DefaultHttpContext().Response);
          
            var httpContext = httpContextMoq.Object;
            httpContext.Response.Body = new MemoryStream();

            var requestDelegate = new RequestDelegate(
                    (innerContext) => throw new Exception("")
            );

            var middleware = new ErrorHandlerMiddleware(requestDelegate);
            await middleware.InvokeAsync(httpContext);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(httpContext.Response.Body).ReadToEnd();

            Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
            Assert.Equal("application/json", httpContext.Response.ContentType);
        }
    }
}
