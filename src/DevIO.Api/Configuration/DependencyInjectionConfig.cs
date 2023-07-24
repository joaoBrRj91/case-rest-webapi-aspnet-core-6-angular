using System;
using DevIO.Api.Extensions;
using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using DevIO.Business.Services;
using DevIO.Data.Context;
using DevIO.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DevIO.Api.Configuration
{
	public static class DependencyInjectionConfig
	{
		public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
		{
            //services.AddAutoMapper(typeof(Program));
            services.AddDbContext<MeuDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultSqlServerConnection"));
            });


            #region Base DI
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<INotificador, Notificador>();
            services.AddScoped<IUser, AspNetUser>();

            #endregion

            #region Repositories
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            #endregion

            #region Services
            services.AddScoped<IFornecedorService, FornecedorService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            #endregion

           
            return services;
		}
	}
}

