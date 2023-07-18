using System;
using System.Security.Claims;
using DevIO.Business.Intefaces;

namespace DevIO.Api.Extensions
{
	public class AspNetUser : IUser
	{
        private readonly IHttpContextAccessor accessor;

        public AspNetUser(IHttpContextAccessor accessor)
		{
            this.accessor = accessor;
        }

        public string Name => accessor.HttpContext!.User.Identity!.Name!;


        public IEnumerable<Claim> GetClaimsIdentity() => accessor.HttpContext!.User.Claims;


        public string GetUserEmail() =>
            IsAuthenticated() ? accessor.HttpContext!.User.GetUserEmail() : string.Empty;


        public Guid GetUserId() =>
            IsAuthenticated() ? Guid.Parse(accessor.HttpContext!.User.GetUser()) : Guid.Empty;


        public bool IsAuthenticated() =>
            accessor.HttpContext!.User.Identity!.IsAuthenticated;


        public bool IsInRole(string role) => accessor.HttpContext!.User.IsInRole(role);

    }


    public static class ClaimsPrincipalExtensions
    {
        public static string GetUser(this ClaimsPrincipal principal)
        {
            if (principal is null)
                throw new ArgumentException(nameof(principal));

            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim!.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if(principal is null)
                throw new ArgumentException(nameof(principal));

            var claim = principal.FindFirst(ClaimTypes.Email);
            return claim!.Value;
        }
    }
}

