using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace HomeSchoolAPI.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private JwtSecurityTokenHandler handler;
        public TokenHelper()
        {
            handler = new JwtSecurityTokenHandler();
        }
        public bool IsValidateToken(string token)
        {
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
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
    }
}