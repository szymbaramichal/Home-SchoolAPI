using System;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Filters;
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
        private Error error;
        public HomeworkController(ITokenHelper tokenHelper, IApiHelper apiHelper)
        {
            error = new Error();
            _tokenHelper = tokenHelper;
            _apiHelper = apiHelper;
        }

        /// <summary>
        /// Adding homework to existing subject in class.
        /// </summary>
        [HttpPost("createHomework")]
        [TokenAuthorization]
        public async Task<IActionResult> AddHomeworkToSubject(HomeworkToAddDTO homeworkToAdd)
        {
            string token = HttpContext.Request.Headers["Authorization"];

            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(homeworkToAdd.ClassID);

            var subject = await _apiHelper.ReturnSubjectBySubjectID(homeworkToAdd.ClassID, homeworkToAdd.SubjectID);

            if(subject == null && classObj.creatorID != id)
            {
                error.Err = "Nie jestes nauczycielem tej klasy";
                error.Desc = "Nie mozesz dodac zadania";
                return StatusCode(405, error);
            }

            try
            {
                var homework = await _apiHelper.AddHomeworkToSubject(subject, homeworkToAdd.Name, homeworkToAdd.Description, homeworkToAdd.Time, homeworkToAdd.FilesID, homeworkToAdd.LinkHrefs);
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


        /// <summary>
        /// Creating response to homework.
        /// </summary>
        [HttpPost("createResponse")]
        [TokenAuthorization]
        public async Task<IActionResult> CreateResponse(ResponseToHomeworkDTO responseToHomework)
        {
            string token = HttpContext.Request.Headers["Authorization"];

            var id = _tokenHelper.GetIdByToken(token);
            var user = await _apiHelper.ReturnUserByID(id);   

            var homework = await _apiHelper.ReturnHomeworkByIDs(responseToHomework.ClassID, responseToHomework.HomeworkID);
            if(homework == null)
            {
                error.Err = "Złe ID zadania lub klasy";
                error.Desc = "Wprowadź poprawne dane";
                return StatusCode(405, error);
            }

            Response response = new Response()
            {
                homeworkID = responseToHomework.HomeworkID,
                senderID = id,
                senderName = user.name,
                senderSurname = user.surrname,
                mark = "",
                description = responseToHomework.Description,
                homeworkName = homework.name,
                sendTime = DateTime.Now,
                files = responseToHomework.FilesID,
                linkHrefs = responseToHomework.LinkHrefs
            };
            var responseReturn = await _apiHelper.CreateResponse(response, responseToHomework.ClassID, homework);
            if(responseReturn == null)
            {
                error.Err = "Nie możesz już oddać zadania";
                error.Desc = "Musisz się pospieszyć na przyszłość";
                return StatusCode(405, error);
            }
            return Ok(responseReturn);
        }
        
        /// <summary>
        /// Deleting homework.
        /// </summary>
        [HttpPut("deleteHomework")]
        [TokenAuthorization]
        public async Task<IActionResult> DeleteHomework([FromBody]DeleteHomeworkDTO deleteHomework)
        {
            string token = HttpContext.Request.Headers["Authorization"];

            var id = _tokenHelper.GetIdByToken(token);
            var subject = await _apiHelper.ReturnSubjectBySubjectID(deleteHomework.ClassID, deleteHomework.SubjectID);
            if(subject.teacherID != id)
            {
                error.Err = "Nie jestes nauczycielem klasy";
                error.Desc = "Nie możesz usunąć przedmiotu";
                return StatusCode(405, error);
            }
            var isDeleted = await _apiHelper.isHomeworkDeleted(deleteHomework.HomeworkID, deleteHomework.SubjectID, deleteHomework.ClassID);
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