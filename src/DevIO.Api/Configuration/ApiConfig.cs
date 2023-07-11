using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Configuration
{
    public static class ApiConfig
    {

        public static IServiceCollection AddWebApiConfig(this IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });

            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "Development",
                    configurePolicy: corsBuilder => corsBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
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

            applicationBuilder.UseAuthorization();

            applicationBuilder.MapControllers();

            applicationBuilder.Run();

            return applicationBuilder;
        }
    }
}

