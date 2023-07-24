using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {

        public static IServiceCollection AddWebApiConfig(this IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddCors(options =>
            {

               /* options.AddDefaultPolicy(
                   configurePolicy: corsBuilder => corsBuilder
                   .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials());*/


                options.AddPolicy(
                    name: "Development",
                    configurePolicy: corsBuilder => corsBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());


                options.AddPolicy(
                    name: "Production",
                    configurePolicy: corsBuilder => corsBuilder
                    .WithMethods("GET", "PUT")
                    .WithOrigins("https://origin-permitida-cors")
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    //.WithHeaders(HeaderNames.ContentType, "Application/json")
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddAutoMapper(typeof(ApiConfig));

            return services;
        }

        public static IApplicationBuilder UseApplicationStartupConfig(this WebApplication applicationBuilder)
        {

            applicationBuilder.UseHttpsRedirection();

            //applicationBuilder.UseCors("Development");

            applicationBuilder.UseAuthorization();

            applicationBuilder.MapControllers();

            applicationBuilder.Run();

            return applicationBuilder;
        }
    }
}

