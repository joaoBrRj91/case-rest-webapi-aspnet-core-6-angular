using System;
using System.Reflection;
using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.ApiVersioning.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/testeVersionApi")]
    public class TesteVersionApiController : MainController
    {
        public TesteVersionApiController(INotificador notificador) : base(notificador)
        {
        }

        public IActionResult AssemblyVersion => Ok(Assembly.GetExecutingAssembly().GetName().Version);

    }
}

