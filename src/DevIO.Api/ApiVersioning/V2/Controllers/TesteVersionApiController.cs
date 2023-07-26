using System;
using System.Reflection;
using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.ApiVersioning.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/testeVersionApi")]
    public class TesteVersionApiController : MainController
    {
        private readonly ILogger<TesteVersionApiController> logger;

        public TesteVersionApiController(INotificador notificador,
            ILogger<TesteVersionApiController> logger) : base(notificador)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult AssemblyVersion()
        {
            throw new Exception("Erro não tratado!");

            //try
            //{
            //    var i = 0;
            //    var result = 42 / i;
            //}
            //catch (dividebyzeroexception ex)
            //{
            //    //todo :envia o httpcontext para o method do elmah que extrai a exception e envia para o service
            //    ex.ship(httpcontext);
            //}


            logger.LogTrace("Log de Trace");
            logger.LogDebug("Log de Debug");
            logger.LogInformation("Log de informação");
            logger.LogWarning("Log de Aviso");
            logger.LogError("Log de Erro");
            logger.LogCritical("Log de problema crítico");


            return Ok(Assembly.GetExecutingAssembly().GetName().Version);
        }

}
}

