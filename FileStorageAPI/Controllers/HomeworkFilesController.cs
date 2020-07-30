using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.ApiResponse;
using HomeSchoolCore.Filters;
using HomeSchoolCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeworkFilesController : ControllerBase
    {
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private Error error;
        public HomeworkFilesController(IApiHelper apiHelper, ITokenHelper tokenHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }

        /// <summary>
        /// Returning file from homework.
        /// </summary>
        [HttpPost("returnFileFromHomework")]
        [TokenAuthorization]
        public async Task<ActionResult> ReturnFileFromHomework(ReturnFileForHomeworkDTO returnForHomework)
        {
            string token = HttpContext.Request.Headers["Authorization"];

            var id = _tokenHelper.GetIdByToken(token);
            var subject = await _apiHelper.ReturnSubjectBySubjectID(returnForHomework.ClassID, returnForHomework.SubjectID);
            if(subject == null)
            {
                error.Err = "Nieprawidlowe ID klasy lub przedmiotu";
                error.Desc = "Wprowadz poprawne wartosci";
                return NotFound(error);
            }
            if(!subject.homeworks.Contains(returnForHomework.HomeworkID) && subject.teacherID != id)
            {
                error.Err = "Nieprawidlowe ID przedmiotu lub nie jestes nauczycielem";
                error.Desc = "Wprowadz poprawne wartosci";
                return NotFound(error);
            }

            var file = await _apiHelper.ReturnHomeworkFileBySenderID(returnForHomework.ClassID, returnForHomework.FileID);
            string asciiEquivalents = Encoding.ASCII.GetString(Encoding.GetEncoding(0).GetBytes(file.fileName));

            Response.Headers.Add("fileName", asciiEquivalents);
            Response.Headers.Remove("Access-Control-Expose-Headers");
            Response.Headers.Add("Access-Control-Expose-Headers", "*");

            return new FileStreamResult(file.stream, file.contentType);
        }

        /// <summary>
        /// Returning file from response to homework.
        /// </summary>
        [HttpPost("returnFileFromResponse")]
        [TokenAuthorization]
        public async Task<ActionResult> ReturnFileFromResponse(ReturnFileForResponseDTO returnForResponse)
        {
            string token = HttpContext.Request.Headers["Authorization"];

            var id = _tokenHelper.GetIdByToken(token);
            var file = await _apiHelper.ReturnResponseFileBySenderID(returnForResponse.HomeworkID, returnForResponse.FileID);
            var classObj = await _apiHelper.ReturnClassByID(returnForResponse.ClassID);
            if(classObj.members.Contains(id))
            {
                var subject = await _apiHelper.ReturnSubjectBySubjectID(returnForResponse.ClassID, returnForResponse.SubjectID);
                if(subject == null)
                {
                    error.Err = "Nieprawidlowe ID klasy lub przedmiotu";
                    error.Desc = "Wprowadz poprawne wartosci";
                    return NotFound(error);
                }
                if(subject.teacherID != id)
                {
                    error.Err = "Nie jestes nauczycielem przedmiotu";
                    error.Desc = "Nie mozesz pobrac odpowiedzi";
                    return NotFound(error);
                }
            }
            else
            {
                error.Err = "Nie nalezysz do klasy";
                error.Desc = "Nie mozesz pobrac pliku";
                return NotFound(error);
            }
            string asciiEquivalents = Encoding.ASCII.GetString(Encoding.GetEncoding(0).GetBytes(file.fileName));
            Response.Headers.Add("fileName", asciiEquivalents);
            Response.Headers.Remove("Access-Control-Expose-Headers");
            Response.Headers.Add("Access-Control-Expose-Headers", "*");
            return new FileStreamResult(file.stream, file.contentType);
        }


        /// <summary>
        /// Uploading file to homework.
        /// </summary>
        [HttpPost("uploadToHomework/{classID}/{subjectID}")]
        [TokenAuthorization]
        public async Task<IActionResult> UploadFileToHomework(string classID, string subjectID, IFormFile file)
        {
            string token = HttpContext.Request.Headers["Authorization"];

            var id = _tokenHelper.GetIdByToken(token);

            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj.members.Contains(id))
            {
                var subject = await _apiHelper.ReturnSubjectBySubjectID(classID, subjectID);
                if(subject == null)
                {
                    error.Err = "Nieprawidlowe id klasy lub nie jestes nauczycielem przedmiotu";
                    error.Desc = "Nie mozesz dodac pliku do zadania";
                    return StatusCode(405, error);
                }
                var fileID = await _apiHelper.UploadFileToHomework(file, classID, id, subjectID);
                FileResponse fileResponse = new FileResponse();
                fileResponse.fileID = fileID;
                return Ok(fileResponse);
            }
            else
            {
                error.Err = "Nie należysz do klasy";
                error.Desc = "Nie mozesz dodac pliku do zadania";
                return StatusCode(405, error);
            }
        }

        /// <summary>
        /// Uploading file to response to existing homework.
        /// </summary>
        [HttpPost("uploadToResponse/{classID}/{homeworkID}")]
        [TokenAuthorization]
        public async Task<IActionResult> UploadFileToResponse(string homeworkID, string classID,IFormFile file)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj.members.Contains(id))
            {
                var fileID = await _apiHelper.UploadFileToResponse(file, homeworkID, id, "", classID);
                if(fileID == null)
                {
                    error.Err = "Złe id zadania";
                    error.Desc = "Nie mozesz dodac pliku do zadania";
                    return StatusCode(405, error);
                }
                FileResponse fileResponse = new FileResponse();
                fileResponse.fileID = fileID;
                return Ok(fileResponse);
            }
            else
            {
                error.Err = "Nie należysz do klasy";
                error.Desc = "Nie mozesz dodac pliku do zadania";
                return StatusCode(405, error);
            }
        }
    
    
    }
}