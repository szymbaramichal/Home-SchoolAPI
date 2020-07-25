using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HomeSchoolCore.Filters
{
    public class TokenAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            TokenHelper tokenHelper = new TokenHelper();
            Error error = new Error();
            string token = context.HttpContext.Request.Headers["Authorization"];
            try
            {
                token = token.Replace("Bearer ", "");
                if (!tokenHelper.IsValidateToken(token))
                {
                    error.Err = "Token wygasł";
                    error.Desc = "Zaloguj się od nowa";
                    context.Result = new BadRequestObjectResult(error);
                }
            }
            catch
            {
                error.Err = "Nieprawidlowy token";
                error.Desc = "Wprowadz token jeszcze raz";
                context.Result = new BadRequestObjectResult(error);
            }         
        }
    }
}