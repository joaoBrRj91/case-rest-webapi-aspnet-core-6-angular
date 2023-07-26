using System;
using DevIO.Api.Extensions;
using Elmah.Io.Extensions.Logging;

namespace DevIO.Api.Configuration
{
	public static class LoggerConfig
	{
        public static IServiceCollection AddLoggingConfig(this IServiceCollection services, IConfiguration configuration)
        {

            //TODO: Descomentar quando configurar o serviço na plataforma Elmah.io
            //services.AddElmahIo(o =>
            //{
            //    o.ApiKey = "API_KEY_ELMAH";
            //    o.LogId = new Guid("GUID_ID_LOG_CONTAINER");
            //});

            /*
            //TODO: Config do provider de log do Elmah.io para enviar os logs manuais para o service de log. Podemos remover esse codigo e utizar um midlleware de exception para enviar erros para o servico
            services.AddLogging(builder =>
            {
                builder.AddElmahIo(o =>
                {
                    o.ApiKey = "API_KEY_ELMAH";
                    o.LogId = new Guid("GUID_ID_LOG_CONTAINER");
                });

                //TODO: LogLevel Information realiza o log de informacoes nao importantes para esse tipo de provider. É interessante realizar os logs de request e exibir em um dashboard kibana para acompanhamento
                builder.AddFilter<ElmahIoLoggerProvider>(category: null, LogLevel.Warning);

            });*/

            //TODO: Descomentar quando configurar o serviço na plataforma Elmah.io
            services.AddHealthChecks()
                 //.AddElmahIoPublisher(options =>
                 //{
                 //    options.ApiKey = "API_KEY_ELMAH";
                 //    options.LogId = new Guid("GUID_ID_LOG_CONTAINER");
                 //    options.HeartbeatId = "API Fornecedores";

                 //})
                 .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultSqlServerConnection")))
                 .AddSqlServer(configuration.GetConnectionString("DefaultSqlServerConnection"), name: "BancoSQL");

            services.AddHealthChecksUI()
                .AddSqlServerStorage(configuration.GetConnectionString("DefaultSqlServerConnection"));


            return services;
        }


        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder application)
        {

            //TODO: Descomentar quando configurar o serviço na plataforma
            //application.UseElmahIo();

            return application;
        }

    }
}

