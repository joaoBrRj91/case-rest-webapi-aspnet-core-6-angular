using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevIO.Api.Configuration;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cors;
using DevIO.Api.Controllers;

namespace DevIO.Api.ApiVersioning.V1.Controllers
{

    [Route("api/auth")]
    // [DisableCors]
    public class AuthController : MainController
    {
        private readonly INotificador notificador;
        private readonly IUser user;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IOptions<AppSettings> options;
        private readonly AppSettings appSettings;

        public AuthController(INotificador notificador,
            IUser user,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> options) : base(notificador)
        {
            this.notificador = notificador;
            this.user = user;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.options = options;
            appSettings = options.Value;
        }


        //TODO: Só funciona se não tiver nenhuma politica de cors configurada em program
        //[EnableCors("Development")]
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
                return CustomResponse(await GerarJwt(usuario.Email));
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
                return CustomResponse(await GerarJwt(loginUsuario.Email));

            if (result.IsLockedOut)
            {
                NotificarErro("Usuario temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(loginUsuario);
            }

            NotificarErro("Usuario ou senha invalidos");
            return CustomResponse(loginUsuario);
        }


        private async Task<string> GerarJwt(string email)
        {

            var user = await userManager.FindByEmailAsync(email);
            var claims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);


            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var role in roles)
                claims.Add(new Claim("Role", role));

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(appSettings.Secret!);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = appSettings.Emissor,
                Audience = appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);
            return encodedToken;
        }


        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}

