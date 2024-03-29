﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrueLayerChallenge.Domain;
using TrueLayerChallenge.WebApi.Models;
using Domain = TrueLayerChallenge.Domain;

namespace TrueLayerChallenge.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class PokemonController : ControllerBase
    {
        private readonly IQueryProcessor _queryProcessor;

        public PokemonController(IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        /// <summary>
        /// Get the pokemon detail
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon</param>
        /// <returns>Pokemon's details</returns>
        [HttpGet]
        [Route("{pokemonName}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPokemon_V1(string pokemonName)
        {
            var query = new Domain.Queries.GetPokemonDetails() { PokemonName = pokemonName };
            var domainResult = await _queryProcessor.ProcessQueryAsync<Domain.Queries.GetPokemonDetails, Domain.QueryModels.Pokemon>(query);
            if (domainResult == null)
            {
                return NotFound($"The requested pokemon - {pokemonName} is not found");
            }

            return Ok(new Pokemon()
            {
                Name = domainResult.Name,
                Description = domainResult.StandardDescription,
                Habitat = domainResult.Habitat,
                IsLegendary = domainResult.IsLegendary
            });
        }
        /// <summary>
        /// Get the pokemon detail with a funny translation
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon</param>
        /// <returns>Pokemon's details</returns>
        [HttpGet]
        [Route("translated/{pokemonName}")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetTranslatedPokemon_V1(string pokemonName)
        {
            var query = new Domain.Queries.GetPokemonTranslatedDetails() { PokemonName = pokemonName };
            var domainResult = await _queryProcessor.ProcessQueryAsync<Domain.Queries.GetPokemonTranslatedDetails, Domain.QueryModels.Pokemon>(query);
            if (domainResult == null)
            {
                return NotFound($"The requested pokemon - {pokemonName} is not found");
            }

            return Ok(new Pokemon()
            {
                Name = domainResult.Name,
                Description = domainResult.TranslatedDescription ?? domainResult.StandardDescription,
                Habitat = domainResult.Habitat,
                IsLegendary = domainResult.IsLegendary
            });
        }
    }
}
