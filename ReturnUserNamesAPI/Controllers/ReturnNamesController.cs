using System;
using System.Threading.Tasks;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using ReturnUserNamesAPI.DTOs;

namespace ReturnUserNamesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReturnNamesController : ControllerBase
    {
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private String token;
        private Error error;
        public ReturnNamesController(IApiHelper apiHelper, ITokenHelper tokenHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }

        [HttpPost]
        public async Task<IActionResult> ReturnNamesFromClass(ReturnNamesDTO returnNames)
        {
            #region TokenValidation
            try
            {
                token = HttpContext.Request.Headers["Authorization"];
                token = token.Replace("Bearer ", "");
                if (!_tokenHelper.IsValidateToken(token))
                {
                    error.Err = "Token wygasł";
                    error.Desc = "Zaloguj się od nowa";
                    return StatusCode(405, error);
                }
            }
            catch
            {
                error.Err = "Nieprawidlowy token";
                error.Desc = "Wprowadz token jeszcze raz";
                return StatusCode(405, error);
            }
            #endregion
            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(returnNames.classID);
            if(classObj == null)
            {
                error.Err = "Złe ID klasy";
                error.Desc = "Wprowadź poprawne ID klasy";
                return StatusCode(405, error);
            }
            if(!classObj.members.Contains(id))
            {
                error.Err = "Nie należysz do klasy";
                error.Desc = "Nie możesz sprawdzić imion i nazwisk";
                return StatusCode(405, error);
            }
            var names = await _apiHelper.ReturnNames(classObj);
            UsersList users = new UsersList 
            {
                users = names
            };
            return Ok(users);
        }
    }
}