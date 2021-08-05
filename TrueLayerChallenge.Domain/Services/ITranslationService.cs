using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayerChallenge.Domain.Services
{
    public interface ITranslationService
    {
        Task<string> GetYodaTranslationAsync(string text);
        Task<string> GetShakespeareTranslationAsync(string text);
    }
}
