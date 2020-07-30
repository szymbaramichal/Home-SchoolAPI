using System;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Filters;
using HomeSchoolCore.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ReturnUserNamesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReturnNamesController : ControllerBase
    {
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private Error error;
        public ReturnNamesController(IApiHelper apiHelper, ITokenHelper tokenHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }
        /// <summary>
        /// Return user names from class
        /// </summary>
        [HttpPost]
        [TokenAuthorization]
        public async Task<IActionResult> ReturnNamesFromClass(ReturnNamesForClassDTO returnNames)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(returnNames.ClassID);
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