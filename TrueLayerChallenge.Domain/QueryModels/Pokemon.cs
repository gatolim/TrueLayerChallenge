using System;
using System.Collections.Generic;
using System.Text;

namespace TrueLayerChallenge.Domain.QueryModels
{
    public class Pokemon
    {
        public string Name { get; set; }
        public string StandardDescription { get; set; }
        public string TranslatedDescription { get; set; }
        public string Habitat { get; set; }
        public bool IsLegendary { get; set; }
    }
}
