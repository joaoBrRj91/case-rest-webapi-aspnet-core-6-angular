using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DevIO.Api.Extensions
{
    public class CustomAuthorization
	{
		public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
			=> context.User.Identity!.IsAuthenticated &&
			context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
	}


    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, string claimValue)
            : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object[] {new Claim(type: claimName, value: claimValue) };
        }
    }

    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        private readonly Claim claim;

        public RequisitoClaimFilter(Claim claim)
        {
            this.claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(!context.HttpContext.User.Identity!.IsAuthenticated)
            {
                context.Result = new StatusCodeResult(401);
                return;
            }

            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, claim.Type, claim.Value))
                context.Result = new StatusCodeResult(403);
        }
    }
}

