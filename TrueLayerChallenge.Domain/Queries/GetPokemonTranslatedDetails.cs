using System;
using System.Collections.Generic;
using System.Text;

namespace TrueLayerChallenge.Domain.Queries
{
    public class GetPokemonTranslatedDetails : IQuery
    {
        public string PokemonName { get; set; }
    }
}
