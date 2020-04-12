using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Data;
using HomeSchoolAPI.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace HomeSchoolAPI.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
                Error error = new Error();
                TokenValidationInfo info = new TokenValidationInfo();
                //getting token from header
                String token = httpContext.Request.Headers["Authorization"];
                
                if(!String.IsNullOrEmpty(token))
                {
                    token = token.Replace("Bearer ", "");
                    //validating token
                    TokenHelper helper = new TokenHelper();
                    try
                    {
                        if(!helper.IsValidateToken(token)) info.IsValidationCorrect = false;
                    } 
                    catch
                    {
                    error.Err = "Nieprawidłowy token";
                    error.Desc = "Podaj poprawny token";
                    }
                }
            
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TokenValidationExtensions
    {
        public static IApplicationBuilder UseTokenValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }
}
