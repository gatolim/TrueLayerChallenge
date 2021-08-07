using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrueLayerChallenge.Domain.Enum;

namespace TrueLayerChallenge.Domain.Services
{
    public interface ITranslationService
    {
        Task<HttpResultResponse<string>> GetTranslationAsync(string text, TranslationType type);
    }
}
