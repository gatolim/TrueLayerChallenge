using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.IO;
using Microsoft.OpenApi.Models;

namespace TrueLayerChallenge.WebApi
{
    public static class SwaggerExtension
    {/// <summary>
     /// Constrcutor
     /// </summary>
     /// <param name="services"></param>
     /// <returns></returns>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1_0", new OpenApiInfo { Title = "TrueLayer Challenge Pokemon Api", Version = "1.0" });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "TrueLayerChallenge.xml"));
            });
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1_0/swagger.json", "TrueLayer Challenge Pokemon Api");
                c.DocExpansion(DocExpansion.List);
            });

            return app;
        }
    }
}
