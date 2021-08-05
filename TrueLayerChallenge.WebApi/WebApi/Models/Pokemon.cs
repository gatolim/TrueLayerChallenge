using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueLayerChallenge.WebApi.Models
{
    public class Pokemon
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Habitat { get; set; }
        public bool IsLegendary { get; set; }
    }
}
