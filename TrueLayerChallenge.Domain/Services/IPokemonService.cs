﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.QueryModels;

namespace TrueLayerChallenge.Domain.Services
{
    public interface IPokemonService
    {
        Task<HttpResultResponse<Pokemon>> GetPokemonDetailsAsync(string text);
    }
}
