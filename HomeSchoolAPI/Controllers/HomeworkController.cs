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

        [HttpPost("add")]
        public async Task<IActionResult> AddHomeworkToSubject(HomeworkToAddDTO homeworkToAddDTO)
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
            var subject = await _apiHelper.ReturnSubjectByTeacherID(homeworkToAddDTO.classID, id);
            if(subject == null)
            {
                error.Err = "Nie jestes nauczycielem tej klasy";
                error.Desc = "Nie mozesz dodac przedmiotu";
                return StatusCode(405, error);
            }
            var homework = await _apiHelper.AddHomeworkToSubject(subject, homeworkToAddDTO.name, homeworkToAddDTO.description);
            return Ok(homework);
            
        }

    }
}