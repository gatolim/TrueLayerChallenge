using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrueLayerChallenge.Domain.Services;

namespace TrueLayerChallenge.Service.TranslationService
{
    public class TranslationService : ITranslationService
    {
        public HttpClient Client { get; }
        private ILogger _logger;

        public TranslationService(HttpClient client, ILogger<TranslationService> logger)
        {
            Client = client;
            Client.BaseAddress = new Uri("https://api.funtranslations.com/");
            _logger = logger;
        }

        public async Task<HttpResultResponse<string>> GetYodaTranslationAsync(string text)
        {
            try
            {
                var input = $"text={HttpUtility.UrlEncode(text)}";

                var jsonResponse = await Client.GetStringAsync($"translate/yoda.json?{input}");

                if (string.IsNullOrWhiteSpace(jsonResponse))
                {
                    return HttpResultResponse<string>.Error("Yoda translation was found.");
                }

                dynamic json = JValue.Parse(jsonResponse);

                return HttpResultResponse<string>.OK(json.contents.translated);
            }
            catch (Exception ex)
            {
                // Might also want to log the pokemon info for troubleshooting.
                _logger.LogError("Failed running Yoda translation");
                return HttpResultResponse<string>.Error($"{ex.Message}");
            }

        }

        public async Task<HttpResultResponse<string>> GetShakespeareTranslationAsync(string text)
        {
            try
            {
                var input = $"text={text}";

                string jsonResponse = await Client.GetStringAsync($"translate/shakespeare.json?{input}");

                if (string.IsNullOrWhiteSpace(jsonResponse))
                {
                    return HttpResultResponse<string>.Error("Shakespeare translation was found.");
                }

                dynamic json = JValue.Parse(jsonResponse);

                return HttpResultResponse<string>.OK(json.contents.translated);
            }
            catch (Exception ex)
            {
                // Might also want to log the pokemon info for troubleshooting.
                _logger.LogError("Failed running Shakespeare translation");
                return HttpResultResponse<string>.Error($"{ex.Message}");
            }
        }
    }
}
