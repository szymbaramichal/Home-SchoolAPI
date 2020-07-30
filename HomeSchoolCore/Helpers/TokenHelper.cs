using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HomeSchoolCore.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        public bool IsValidateToken(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            SecurityToken validatedToken;
            var validation = handler.ValidateToken(token, GetParametersToValidate(), out validatedToken);
            DateTime tokendate = jsonToken.ValidTo;
            double dateNow = (DateTime.Now - DateTime.UnixEpoch).TotalMinutes;
            double tokenDate = (tokendate - DateTime.UnixEpoch).TotalMinutes;
            if(dateNow - tokenDate < 0)
            {
                return true;
            }
            else return false;
        }

        public string GetIdByToken(string token) 
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var id = jsonToken.Claims.First(claim => claim.Type == "nameid").Value;
            return id;
        }
        private TokenValidationParameters GetParametersToValidate()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AppSettingsHelper.tokenKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
    }
}