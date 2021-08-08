using System;
using Xunit;
using RichardSzalay.MockHttp;
using System.Net.Http;
using Moq;
using Microsoft.Extensions.Logging;
using System.Web;
using Newtonsoft.Json.Linq;
using TrueLayerChallenge.Domain.Services;
using TrueLayerChallenge.Domain.Enum;

namespace TrueLayerChallenge.Service.TranslationService.Test
{
    public class TranslationServiceTest
    {
        private MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
        private HttpClient _httpClient;
        private Mock<ILogger<TranslationService>> logger = new Mock<ILogger<TranslationService>>();
        private TranslationService _service;

        [Theory]
        [InlineData(TranslationType.Yoda, "YodaTranslatedText")]
        [InlineData(TranslationType.Shakespeare, "ShakespeareTranslatedText")]
        public async void TestSucceedFromApi(TranslationType type, string expectedText)
        {
            var untranslatedText = "test";
            var encodedText = $"text={HttpUtility.UrlEncode(untranslatedText)}";

            dynamic dynamicYodaResponse = new JObject() as dynamic;
            dynamicYodaResponse.contents = new JObject() as dynamic;
            dynamicYodaResponse.contents.translated = "YodaTranslatedText";
            string mockYodaResponse = dynamicYodaResponse.ToString();

            dynamic dynamicShakespeareResponse = new JObject() as dynamic;
            dynamicShakespeareResponse.contents = new JObject() as dynamic;
            dynamicShakespeareResponse.contents.translated = "ShakespeareTranslatedText";
            string mockShakespeareResponse = dynamicShakespeareResponse.ToString();

            // Setup a respond for the translation api
            mockHttp.When($"https://api.funtranslations.com/translate/yoda.json?{encodedText}")
            .Respond("application/json", mockYodaResponse); // Respond with JSON

            mockHttp.When($"https://api.funtranslations.com/translate/shakespeare.json?{encodedText}")
            .Respond("application/json", mockShakespeareResponse); // Respond with JSON


            _httpClient = new HttpClient(mockHttp);
            _service = new TranslationService(_httpClient, logger.Object);
            HttpResultResponse<string> response = await _service.GetTranslationAsync(untranslatedText, type);

            Assert.True(response.IsSucceed);
            Assert.NotNull(response.Data);
            Assert.Equal(expectedText, response.Data);
        }

        [Fact]
        public async void TestNoResponseFromApi()
        {
            var untranslatedText = "test";
            var encodedText = $"text={HttpUtility.UrlEncode(untranslatedText)}";
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"https://api.funtranslations.com/translate/yoda.json?{encodedText}")
            .Respond("application/json", ""); // Respond with JSON

            _httpClient = new HttpClient(mockHttp);
            _service = new TranslationService(_httpClient, logger.Object);
            HttpResultResponse<string> response = await _service.GetTranslationAsync(untranslatedText, TranslationType.Yoda);

            Assert.False(response.IsSucceed);
            Assert.Null(response.Data);
            Assert.Equal("Translation was not found.", response.ErrorMessage);
        }

        [Fact]
        public async void TestErrorFromApi()
        {
            var untranslatedText = "test";
            var encodedText = $"text={HttpUtility.UrlEncode(untranslatedText)}";
            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"https://api.funtranslations.com/translate/yoda.json?{encodedText}")
                .Throw(new HttpRequestException());

            _httpClient = new HttpClient(mockHttp);
            _service = new TranslationService(_httpClient, logger.Object);
            HttpResultResponse<string> response = await _service.GetTranslationAsync(untranslatedText, TranslationType.Yoda);

            Assert.False(response.IsSucceed);
            Assert.Null(response.Data);
        }
    }
}
