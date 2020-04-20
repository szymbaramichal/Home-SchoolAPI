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
    public class SubjectController : ControllerBase
    {
        private Error error;
        private String token;
        private readonly IUserHelper _userHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly IClassHelper _classHelper;
        private readonly ISubjectHelper _subjectHelper;
        public SubjectController(IUserHelper userHelper, ITokenHelper tokenHelper, IClassHelper classHelper, ISubjectHelper subjectHelper)
        {
            error = new Error();
            _classHelper = classHelper;
            _userHelper = userHelper;
            _tokenHelper = tokenHelper;
            _subjectHelper = subjectHelper;
        }
 
        [HttpPost]
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

            var user = await _userHelper.ReturnUserByMail(createSubjectDTO.userToAddEmail);

            if(user == null || user.userRole == 0)
            {
                error.Err = "Nie poprawny nauczyciel";
                error.Desc = "Wprowadz email nauczyciela jeszcze raz";
                return StatusCode(405, error);
            }

            var classForUser = await _classHelper.ReturnClassByID(createSubjectDTO.classID);

            if(classForUser == null)
            {
                error.Err = "Nie ma takiej klasy pajacu";
                error.Desc = "Wprowadź ID klasy jeszcze raz";
                return StatusCode(405, error);
            }

            var subject = await _subjectHelper.AddSubjectToClass(user.Id, classForUser, createSubjectDTO.subjectName);

            if(subject == null)
            {
                error.Err = "Już istnieje taki przedmiot";
                error.Desc = "Zmień nazwę przedmiotu";
                return StatusCode(405, error);
            }

            return Ok(subject);
        }
    }
}