using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Queries;

namespace TrueLayerChallenge.Domain.QueryHandlers
{
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery
    {
        Task<TResult> ReadAsync(TQuery query);
    }
}
