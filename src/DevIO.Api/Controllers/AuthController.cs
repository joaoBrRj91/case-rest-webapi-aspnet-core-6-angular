using System;
using System.IdentityModel.Tokens.Jwt;
using DevIO.Api.Configuration;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DevIO.Api.Controllers
{

    [Route("api/auth")]
    public class AuthController : MainController
    {
        private readonly INotificador notificador;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppSettings appSettings;

        public AuthController(INotificador notificador,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> options) : base(notificador)
        {
            this.notificador = notificador;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appSettings = options.Value;
        }


        [HttpPost]
        [Route("nova-conta")]
        public async Task<ActionResult> Registrar(RegistroUsuarioViewModel registroUsuario)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var usuario = new IdentityUser
            {
                UserName = registroUsuario.Email,
                Email = registroUsuario.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(usuario, registroUsuario.Senha);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: false);
                return CustomResponse(GerarJwt());
            }

            foreach (var erro in result.Errors)
                NotificarErro(erro.Description);

            return CustomResponse(registroUsuario);
        }


        [HttpPost]
        [Route("entrar")]
        public async Task<ActionResult> Login(LoginUsuarioViewModel loginUsuario)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await signInManager
                .PasswordSignInAsync(loginUsuario.Email,
                loginUsuario.Senha,
                isPersistent: false,
                lockoutOnFailure: true);

            if (result.Succeeded)
                return CustomResponse(GerarJwt());

            if(result.IsLockedOut)
            {
                NotificarErro("Usuario temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(loginUsuario);
            }

            NotificarErro("Usuario ou senha invalidos");
            return CustomResponse(loginUsuario);
        }


        private string GerarJwt()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(appSettings.Secret!);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = appSettings.Emissor,
                Audience = appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);
            return encodedToken;
        }
    }
}

