using System;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.DTOs;
using HomeSchoolAPI.Helpers;
using HomeSchoolAPI.Models;
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
            var classObj = await _apiHelper.ReturnClassByID(homeworkToAddDTO.classID);

            var subject = await _apiHelper.ReturnSubjectByTeacherID(homeworkToAddDTO.classID, id);

            if(subject == null && classObj.creatorID != id)
            {
                error.Err = "Nie jestes nauczycielem tej klasy";
                error.Desc = "Nie mozesz dodac zadania";
                return StatusCode(405, error);
            }
            var homework = await _apiHelper.AddHomeworkToSubject(subject, homeworkToAddDTO.name, homeworkToAddDTO.description, homeworkToAddDTO.time);
            return Ok(homework);
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
            Response response = new Response()
            {
                homeworkID = responseToHomework.homeworkID,
                senderID = id,
                senderName = user.name,
                senderSurname = user.surrname,
                mark = "",
                description = responseToHomework.description,
                sendTime = DateTime.Now,
            };
            var homework = await _apiHelper.CreateResponse(response, responseToHomework.classID);
            if(homework == null)
            {
                error.Err = "Nie możesz już oddać zadania";
                error.Desc = "Musisz się pospieszyć na przyszłość";
                return StatusCode(405, error);
            }
            return Ok(homework);
        }

    }
}