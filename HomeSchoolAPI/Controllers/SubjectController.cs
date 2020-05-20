using System;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace HomeSchoolAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private Error error;
        private String token;
        private readonly ITokenHelper _tokenHelper;
        private readonly IApiHelper _apiHelper;
        public SubjectController(ITokenHelper tokenHelper, IApiHelper apiHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }
 
        [HttpPost("create")]
        public async Task<IActionResult> CreateSubject(CreateSubjectDTO createSubjectDTO)
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

            var user = await _apiHelper.ReturnUserByMail(createSubjectDTO.userToAddEmail);

            if(user == null || user.userRole == 0)
            {
                error.Err = "Nie poprawny nauczyciel";
                error.Desc = "Wprowadz email nauczyciela jeszcze raz";
                return StatusCode(405, error);
            }

            var classObj = await _apiHelper.ReturnClassByID(createSubjectDTO.classID);

            if(classObj == null)
            {
                error.Err = "Nie ma takiej klasy";
                error.Desc = "Wprowadź ID klasy jeszcze raz";
                return StatusCode(405, error);
            }

            if(classObj.creatorID != id)
            {
                error.Err = "Nie jestes wychowawcą";
                error.Desc = "Nie możesz tworzyć przedmiotu";
                return StatusCode(405, error);
            }

            var subject = await _apiHelper.AddSubjectToClass(user.Id, classObj, createSubjectDTO.subjectName);
            
            await _apiHelper.ReplaceClassInfo(subject.classObj);

            return Ok(subject);
        }
    }
}