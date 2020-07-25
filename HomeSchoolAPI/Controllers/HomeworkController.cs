using System;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Helpers;
using HomeSchoolCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeSchoolAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeworkController : ControllerBase
    {
        private ITokenHelper _tokenHelper;
        private IApiHelper _apiHelper;
        private String token;
        private Error error;
        public HomeworkController(ITokenHelper tokenHelper, IApiHelper apiHelper)
        {
            error = new Error();
            _tokenHelper = tokenHelper;
            _apiHelper = apiHelper;
        }

        [HttpPost("createHomework")]
        public async Task<IActionResult> AddHomeworkToSubject(HomeworkToAddDTO homeworkToAdd)
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
            var classObj = await _apiHelper.ReturnClassByID(homeworkToAdd.classID);

            var subject = await _apiHelper.ReturnSubjectBySubjectID(homeworkToAdd.classID, homeworkToAdd.subjectID);

            if(subject == null && classObj.creatorID != id)
            {
                error.Err = "Nie jestes nauczycielem tej klasy";
                error.Desc = "Nie mozesz dodac zadania";
                return StatusCode(405, error);
            }

            try
            {
                var homework = await _apiHelper.AddHomeworkToSubject(subject, homeworkToAdd.name, homeworkToAdd.description, homeworkToAdd.time, homeworkToAdd.filesID, homeworkToAdd.linkHrefs);
                if(homework == null)
                {
                    error.Err = "Złe ID pliku";
                    error.Desc = "Nie mozesz dodac zadania";
                    return StatusCode(405, error);
                }
                return Ok(homework);
            }
            catch
            {
                error.Err = "Błędne dane";
                error.Desc = "Nie mozesz dodac zadania";
                return StatusCode(405, error);
            }
        }

        [HttpPost("createResponse")]
        public async Task<IActionResult> CreateResponse(ResponseToHomeworkDTO responseToHomework)
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
            var id = _tokenHelper.GetIdByToken(token);
            var user = await _apiHelper.ReturnUserByID(id);   
            #endregion
            var homework = await _apiHelper.ReturnHomeworkByIDs(responseToHomework.classID, responseToHomework.homeworkID);
            if(homework == null)
            {
                error.Err = "Złe ID zadania lub klasy";
                error.Desc = "Wprowadź poprawne dane";
                return StatusCode(405, error);
            }

            Response response = new Response()
            {
                homeworkID = responseToHomework.homeworkID,
                senderID = id,
                senderName = user.name,
                senderSurname = user.surrname,
                mark = "",
                description = responseToHomework.description,
                homeworkName = homework.name,
                sendTime = DateTime.Now,
                files = responseToHomework.filesID,
                linkHrefs = responseToHomework.linkHrefs
            };
            var responseReturn = await _apiHelper.CreateResponse(response, responseToHomework.classID, homework);
            if(responseReturn == null)
            {
                error.Err = "Nie możesz już oddać zadania";
                error.Desc = "Musisz się pospieszyć na przyszłość";
                return StatusCode(405, error);
            }
            return Ok(responseReturn);
        }
        [HttpPut("deleteHomework")]
        public async Task<IActionResult> DeleteHomework([FromBody]DeleteHomeworkDTO deleteHomework)
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
            var subject = await _apiHelper.ReturnSubjectBySubjectID(deleteHomework.classID, deleteHomework.subjectID);
            if(subject.teacherID != id)
            {
                error.Err = "Nie jestes nauczycielem klasy";
                error.Desc = "Nie możesz usunąć przedmiotu";
                return StatusCode(405, error);
            }
            var isDeleted = await _apiHelper.isHomeworkDeleted(deleteHomework.homeworkID, deleteHomework.subjectID, deleteHomework.classID);
            if(isDeleted)
            {
                error.Err = "Pomyślnie usunięto zadanie";
                error.Desc = "Udało się usunąć zadanie";
                return StatusCode(200, error);
            }
            else
            {
                error.Err = "Nie jestes nauczycielem klasy";
                error.Desc = "Nie możesz usunąć przedmiotu";
                return StatusCode(405, error);
            }
        }
    }
}