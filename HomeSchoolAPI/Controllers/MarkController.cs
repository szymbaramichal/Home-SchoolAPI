using System;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.DTOs;
using HomeSchoolAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace HomeSchoolAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarkController : ControllerBase
    {
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private String token;
        private Error error;
        public MarkController(IApiHelper apiHelper, ITokenHelper tokenHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }

        [HttpPut]
        public async Task<IActionResult> PutMark([FromBody] PutMarkDTO putMark)
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
            
            var subject = await _apiHelper.ReturnSubjectByTeacherID(putMark.classID, id);
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