using System;
using Elmah.Io.Extensions.Logging;

namespace DevIO.Api.Configuration
{
	public static class LoggerConfig
	{
        public static IServiceCollection AddLoggingConfig(this IServiceCollection services)
        {

            services.AddElmahIo(o =>
            {
                o.ApiKey = "API_KEY_ELMAH";
                o.LogId = new Guid("GUID_ID_LOG_CONTAINER");
            });

            /*
            //TODO: Config do provider de log do Elmah.io para enviar os logs manuais para o service de log
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

            return services;
        }


        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder application)
        {

            application.UseElmahIo();

            return application;
        }

    }
}

