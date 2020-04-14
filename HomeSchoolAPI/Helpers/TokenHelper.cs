using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HomeSchoolAPI.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _configuration;
        private JwtSecurityTokenHandler handler;
        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            handler = new JwtSecurityTokenHandler();
        }
        public bool IsValidateToken(string token)
        {
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            SecurityToken validatedToken;
            var validation = handler.ValidateToken(token, GetParametersToValidate(), out validatedToken);
            DateTime tokendate = jsonToken.ValidTo;
            double dateNow = (DateTime.Now - DateTime.UnixEpoch).TotalSeconds;
            double tokenDate = (tokendate - DateTime.UnixEpoch).TotalSeconds;
            if(dateNow - tokenDate < 0)
            {
                return true;
            }
            else return false;
        }

        public string GetIdByToken(string token) 
        {
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var id = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;
            return id;
        }
        private TokenValidationParameters GetParametersToValidate()
        {
                return new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
        }
    }
}