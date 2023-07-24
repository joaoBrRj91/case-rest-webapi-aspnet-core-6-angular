using System;
using System.Reflection;
using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.ApiVersioning.V1.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/v{version:apiVersion}/testeVersionApi")]
    public class TesteVersionApiController : MainController
    {
        public TesteVersionApiController(INotificador notificador) : base(notificador)
        {
        }


        public IActionResult AssemblyVersion => Ok(Assembly.GetExecutingAssembly().GetName().Version);
    }
}

