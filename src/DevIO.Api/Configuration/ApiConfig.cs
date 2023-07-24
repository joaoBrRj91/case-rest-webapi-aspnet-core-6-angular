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


            #region API Versioning
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            #endregion

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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

            services.AddAutoMapper(typeof(ApiConfig));

            return services;
        }

        public static IApplicationBuilder UseApplicationStartupConfig(this WebApplication applicationBuilder)
        {

            applicationBuilder.UseHttpsRedirection();

            applicationBuilder.UseCors("Development");

            applicationBuilder.UseAuthentication();
            applicationBuilder.UseAuthorization();

            applicationBuilder.MapControllers();

            applicationBuilder.Run();

            return applicationBuilder;
        }
    }
}

