using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.QueryModels;

namespace TrueLayerChallenge.Domain.Services
{
    public interface IHttpResultResponse<T> where T : class
    {
        HttpResultResponse<T> OK(T data);
        HttpResultResponse<T> Error(string message);
    }
}
