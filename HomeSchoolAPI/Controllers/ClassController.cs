using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.DTOs;
using HomeSchoolAPI.Helpers;
using HomeSchoolAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSchoolAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private ITokenHelper _tokenHelper;
        private IApiHelper _apiHelper;
        private String token;
        private Error error;
        public ClassController(ITokenHelper tokenHelper, IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
            error = new Error();
            _tokenHelper = tokenHelper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateClass(ClassToCreateDTO classToCreate)
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

            if(String.IsNullOrWhiteSpace(classToCreate.className) || String.IsNullOrWhiteSpace(classToCreate.schoolName))
            {
                error.Err = "Uzupełnij wszystkie pola";
                error.Desc = "Żadne z pól nie może zostać puste";
                return StatusCode(405, error);  
            }

            User creator = new User();   

                List<string> list1 = new List<string>();
                var id = _tokenHelper.GetIdByToken(token);
                creator = await _apiHelper.ReturnUserByID(id);
                if(creator == null)
                {
                    error.Err = "Nieprawidlowy token";
                    error.Desc = "Wprowadz token jeszcze raz";
                    return StatusCode(405, error);
                }

                if(creator.userRole == 1)
                {
                    var createdClass = await _apiHelper.CreateClass(creator, classToCreate.className, classToCreate.schoolName);
                    return Ok(createdClass);
                }
                else
                {
                    error.Err = "Nie jestes nauczycielem";
                    error.Desc = "Nie mozesz zalozyc klasy";
                    return StatusCode(409, error);
                }
        }
    
        [HttpPut("addMember")]
        public async Task<IActionResult> AddMemberToClass([FromBody]AddToClassDTO addToClassDTO)
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
            Class classe = new Class();
            List<string> list1 = new List<string>();
            var id = _tokenHelper.GetIdByToken(token);
            var teacher = await _apiHelper.ReturnUserByID(id);
            if(teacher.userRole != 1)
            {
                error.Err = "Nie możesz dodać ucznia do klasy";
                error.Desc = "Nie jesteś nauczycielem";
                return StatusCode(405, error);
            }
            
            classe = await _apiHelper.ReturnClassByID(addToClassDTO.ClassID);

            if(classe == null)
            {
                error.Err = "ROBIĘ ŹLE A CHCIAŁEM DOBRZE";
                error.Desc = "Wprowadź poprawne ID klasy";
                return StatusCode(409, error);
            }

            if(_apiHelper.DoesUserExistByEmail(addToClassDTO.UserToAddEmail))
            {
                var classToReturn = await _apiHelper.AddMemberToClass(addToClassDTO.UserToAddEmail, classe);
                if(classToReturn == null)
                {
                    error.Err = "Ten uczeń już jest w tej klasie";
                    error.Desc = "Nie musisz dodawać tego użytkownika";
                    return StatusCode(409, error);
                }
                return Ok(classToReturn);
            }
            else
            {
                error.Err = "Niepoprawny email uzytkownika";
                error.Desc = "Wprowadz poprawne email uzytkownika";
                return StatusCode(409, error);
            }
        }

    }
}