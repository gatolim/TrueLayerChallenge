using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrueLayerChallenge.Domain.Enum;
using TrueLayerChallenge.Domain.Services;

namespace TrueLayerChallenge.Service.TranslationService
{
    /// <summary>
    /// A simple service that wrapped with an HttpClient, which in used to interact with external endpoint.
    /// </summary>
    public class TranslationService : ITranslationService
    {
        public HttpClient Client { get; }
        private ILogger _logger;
        protected IDictionary<TranslationType, string> _translationTypeMapping;

        public TranslationService(HttpClient client, ILogger<TranslationService> logger)
        {
            Client = client;
            Client.BaseAddress = new Uri("https://api.funtranslations.com/");
            _logger = logger;

            // Might want to move this to a configuration or something. 
            _translationTypeMapping = new Dictionary<TranslationType, string>() { 
                {TranslationType.Yoda, "translate/yoda.json" }, 
                {TranslationType.Shakespeare , "translate/shakespeare.json" } }
            ;
        }

        public async Task<HttpResultResponse<string>> GetTranslationAsync(string text, TranslationType type)
        {
            try
            {
                var input = $"text={HttpUtility.UrlEncode(text)}";
                string endpoint;
                if (!_translationTypeMapping.TryGetValue(type, out endpoint))
                {
                    return HttpResultResponse<string>.Error("Translation was not found.");
                }
                string query = $"{endpoint}?{input}";

                var jsonResponse = await Client.GetStringAsync(query);

                if (string.IsNullOrWhiteSpace(jsonResponse))
                {
                    return HttpResultResponse<string>.Error("Translation was not found.");
                }

                // Prase result into dynamic so we don't need to maintain a model. 
                // This should be sufficient for this purpose of this exercise.
                dynamic json = JValue.Parse(jsonResponse);
                string translatedText = json.contents.translated;

                return HttpResultResponse<string>.OK(translatedText);
            }
            catch (Exception ex)
            {
                // Might also want to log the pokemon info for troubleshooting.
                _logger.LogError("Failed getting translation");
                return HttpResultResponse<string>.Error($"{ex.Message}");
            }

        }
    }
}
