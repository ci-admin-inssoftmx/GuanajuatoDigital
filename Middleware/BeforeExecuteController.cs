
using GuanajuatoAdminUsuarios.LoginController;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Middleware
{


    public class BeforeExecuteController 
    {
        private readonly RequestDelegate _next;

        public BeforeExecuteController(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {

            var authService = context.RequestServices.GetRequiredService<IAuthorizationService>();

            var GUID2 = context.User.FindFirst(CustomClaims.GUID);

            string GUID = GUID2 != null ? GUID2.Value : "";



            if(!string.IsNullOrEmpty(GUID))
            {
                var User = AuthManager.GetUser(GUID);
                if(User != null && !User.CanUse)
                {
                    context.SignOutAsync().Wait();
                    context.Response.Redirect("");
                    AuthManager.GetUser(GUID);
                }                
            }



            await _next(context);
        }

    }

    public static class BeforeExecuteControllerExtensions
    {
        public static IApplicationBuilder UseBeforeController(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BeforeExecuteController>();
        }
    }
}





namespace GuanajuatoAdminUsuarios.TestEvents
{

    public class CokkieEventsCustom : CookieAuthenticationEvents
    {


        public override Task SignedIn(CookieSignedInContext context)
        {
            var t = base.SignedIn(context);
            var tt= context.HttpContext.User;
            Console.WriteLine("Im here After Loggin");

            return t; 
        }


        public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var t = base.ValidatePrincipal(context);
            return t;
        }


    }


}


