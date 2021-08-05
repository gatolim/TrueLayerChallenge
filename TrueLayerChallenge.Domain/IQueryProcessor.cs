using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Queries;

namespace TrueLayerChallenge.Domain
{
    public interface IQueryProcessor
    {
        Task<TResult> ProcessQueryAsync<TQuery, TResult>(TQuery query)
               where TQuery : IQuery;
    }
}
