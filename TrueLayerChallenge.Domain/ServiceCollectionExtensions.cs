using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TrueLayerChallenge.Domain.Queries;
using TrueLayerChallenge.Domain.QueryHandlers;
using TrueLayerChallenge.Domain.QueryModels;

namespace TrueLayerChallenge.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDomain(this IServiceCollection serviceCollection)
        {
            RegisterQueryHandlers(serviceCollection);
        }

        private static void RegisterQueryHandlers(IServiceCollection services)
        {
            services.AddTransient<IQueryProcessor, QueryProcessor>();
            services.AddTransient<IQueryHandler<GetPokemonDetails, Pokemon>, GetPokemonHandler>();
            services.AddTransient<IQueryHandler<GetPokemonTranslatedDetails, Pokemon>, GetPokemonTranslatedHandler>();
        }
    }
}
