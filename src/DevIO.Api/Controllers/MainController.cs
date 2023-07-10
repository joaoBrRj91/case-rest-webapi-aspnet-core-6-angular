using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.Api.Controllers
{

    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador notificador;

        protected MainController(INotificador notificador)
        {
            this.notificador = notificador;
        }


        protected bool OperacaoValida() => !notificador.TemNotificacao();


        protected ActionResult CustomResponse(object? result = null)
        {

            if (OperacaoValida())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = notificador.ObterNotificacoes().Select(n=>n.Mensagem)
            });

        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = ModelState.Values.SelectMany(e => e.Errors);

            foreach (var erro in erros)
            {
                var erroMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(erroMsg);
            }
        }

        protected void NotificarErro(string erro)
        {
            notificador.Handle(new Notificacao(erro));
        }
    }
}

