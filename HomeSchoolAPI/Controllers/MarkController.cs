using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Filters;
using HomeSchoolCore.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace HomeSchoolAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarkController : ControllerBase
    {
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private Error error;
        public MarkController(IApiHelper apiHelper, ITokenHelper tokenHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }

        [HttpPut]
        [TokenAuthorization]
        public async Task<IActionResult> PutMark([FromBody] PutMarkDTO putMark)
        {
            string token = HttpContext.Request.Headers["Authorization"];

            var id = _tokenHelper.GetIdByToken(token);
            
            var subject = await _apiHelper.ReturnSubjectBySubjectID(putMark.classID, putMark.subjectID);
            if(subject == null)
            {
                error.Err = "Nie jesteś nauczycielem przedmiotu";
                error.Desc = "Nie możesz ocenić zadania";
                return StatusCode(405, error);
            }
            if(!subject.homeworks.Contains(putMark.homeworkID))
            {
                error.Err = "Niepoprawne ID zadania";
                error.Desc = "Wprowadz zadanie ponownie";
                return StatusCode(405, error);
            }

            var response = await _apiHelper.PutMark(putMark.homeworkID, putMark.responseID, putMark.mark);
            if(response == null)
            {
                error.Err = "Niepoprawne ID odpowiedzi";
                error.Desc = "Wprowadz ID odpowiedzi ponownie";
                return StatusCode(405, error);
            }
            return Ok(response);

        }
    }
}