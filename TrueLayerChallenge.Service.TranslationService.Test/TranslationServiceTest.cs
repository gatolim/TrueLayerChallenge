using System;
using Xunit;
using RichardSzalay.MockHttp;
using System.Net.Http;
using TrueLayerChallenge.Domain.Services;
using Moq;
using Microsoft.Extensions.Logging;
using System.Web;
using Newtonsoft.Json.Linq;

namespace TrueLayerChallenge.Service.TranslationService.Test
{
    public class TranslationServiceTest
    {
        private MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
        private HttpClient _httpClient;
        private Mock<ILogger<TranslationService>> logger = new Mock<ILogger<TranslationService>>();
        private ITranslationService _service;

        [Fact]
        public async void TestSucceedFromApi()
        {
            var untranslatedText = "test";
            var encodedText = $"text={HttpUtility.UrlEncode(untranslatedText)}";

            dynamic dynamicResponse = new JObject() as dynamic;
            dynamicResponse.contents = new JObject() as dynamic;
            dynamicResponse.contents.translated = "SomeTranslatedText";

            string mockResponse = dynamicResponse.ToString();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"https://api.funtranslations.com/translate/yoda.json?{encodedText}")
            .Respond("application/json", mockResponse); // Respond with JSON


            _httpClient = new HttpClient(mockHttp);
            _service = new TranslationService(_httpClient, logger.Object);
            HttpResultResponse<string> response = await _service.GetYodaTranslationAsync(untranslatedText);

            Assert.True(response.IsSucceed);
            Assert.NotNull(response.Data);
            Assert.Equal("SomeTranslatedText", response.Data);
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
            HttpResultResponse<string> response = await _service.GetYodaTranslationAsync(untranslatedText);

            Assert.False(response.IsSucceed);
            Assert.Null(response.Data);
            Assert.Equal("Yoda translation was found.", response.ErrorMessage);
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
            HttpResultResponse<string> response = await _service.GetYodaTranslationAsync(untranslatedText);

            Assert.False(response.IsSucceed);
            Assert.Null(response.Data);
        }
    }
}
