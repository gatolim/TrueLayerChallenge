using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Queries;
using TrueLayerChallenge.Domain.QueryHandlers;

namespace TrueLayerChallenge.Domain
{
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryProcessor" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public QueryProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Processes the query asynchronously.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>The relevant query response.</returns>
        public Task<TResult> ProcessQueryAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery
        {
            return _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>().ReadAsync(query);
        }
    }
}
