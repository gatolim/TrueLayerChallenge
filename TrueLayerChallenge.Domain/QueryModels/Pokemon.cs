﻿using System;
using System.Collections.Generic;
using System.Text;
using TrueLayerChallenge.Domain.Enum;

namespace TrueLayerChallenge.Domain.QueryModels
{
    public class Pokemon
    {
        public string Name { get; set; }
        public string StandardDescription { get; set; }
        public string TranslatedDescription { get; set; }
        public string Habitat { get; set; }
        public bool IsLegendary { get; set; }
        public TranslationType TranslationType
        {
            get
            {
                if ((!string.IsNullOrWhiteSpace(this.Habitat) && this.Habitat.Equals("cave", StringComparison.OrdinalIgnoreCase)) || this.IsLegendary)
                {
                    return TranslationType.Yoda;
                }
                else
                {
                    return TranslationType.Shakespeare;
                };
            }
        }
    }
}
