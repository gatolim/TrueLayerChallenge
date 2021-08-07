using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Enum;
using TrueLayerChallenge.Domain.QueryModels;
using Xunit;

namespace TrueLayerChallenge.Domain.Test
{
    public class PokemonModelTest
    {
        [Theory]
        [InlineData("cave", true, TranslationType.Yoda)]
        [InlineData("cave", false, TranslationType.Yoda)]
        [InlineData("notcave", true, TranslationType.Yoda)]
        [InlineData("", true, TranslationType.Yoda)]
        [InlineData("notcave", false, TranslationType.Shakespeare)]
        [InlineData("", false, TranslationType.Shakespeare)]
        public void GetYodaTranslationType(string habitat, bool isLegendary, TranslationType expectedType)
        {
            Pokemon pokemon = new Pokemon()
            {
                Name = "TestPokemon",
                Habitat = habitat,
                IsLegendary = isLegendary
            };
            Assert.Equal(expectedType, pokemon.TranslationType);
        }
    }
}
