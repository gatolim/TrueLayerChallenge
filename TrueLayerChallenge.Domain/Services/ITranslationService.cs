using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayerChallenge.Domain.Services
{
    public interface ITranslationService
    {
        Task<HttpResultResponse<string>> GetYodaTranslationAsync(string text);
        Task<HttpResultResponse<string>> GetShakespeareTranslationAsync(string text);
    }
}
