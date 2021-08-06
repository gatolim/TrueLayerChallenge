using Microsoft.Extensions.DependencyInjection;
using TrueLayerChallenge.Domain.Services;

namespace TrueLayerChallenge.Service.TranslationService
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTranslationService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient<ITranslationService, TranslationService>();
        }
    }
}
