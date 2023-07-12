using System;
using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using DevIO.Business.Services;
using DevIO.Data.Context;
using DevIO.Data.Repository;
using Microsoft.EntityFrameworkCore;

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
            services.AddScoped<INotificador, Notificador>();
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

