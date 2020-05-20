using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Helpers;
using HomeSchoolCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeSchoolAPI.Controllers
{
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
            Class classObj = new Class();
            List<string> list1 = new List<string>();

            var id = _tokenHelper.GetIdByToken(token);
            
            classObj = await _apiHelper.ReturnClassByID(addToClassDTO.ClassID);

            if(classObj == null)
            {
                error.Err = "Niepoprawne ID klasy";
                error.Desc = "Wprowadź poprawne ID klasy";
                return StatusCode(409, error);
            }

            if(classObj.creatorID != id)
            {
                error.Err = "Nie jesteś wychowawcą klasy";
                error.Desc = "Nie możesz dodać użytkownika";
                return StatusCode(409, error);
            }

            if(_apiHelper.DoesUserExistByEmail(addToClassDTO.UserToAddEmail))
            {
                var classToReturn = await _apiHelper.AddMemberToClass(addToClassDTO.UserToAddEmail, classObj);
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

        [HttpPut("deleteMember")]
        public async Task<IActionResult> DeleteMember([FromBody]DeleteMemberDTO deleteMemberDTO)
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
            var classObj = await _apiHelper.ReturnClassByID(deleteMemberDTO.classID);
            if(classObj == null)
            {
                error.Err = "Niepoprawne ID klasy";
                error.Desc = "Nie możesz usunąć członka klasy";
                return StatusCode(409, error);
            }
            if(!classObj.members.Contains(deleteMemberDTO.userToDeleteID))
            {
                error.Err = "Użytkownik nie należy do klasy";
                error.Desc = "Nie możesz usunąć członka klasy";
                return StatusCode(409, error);
            }
            if(classObj.creatorID != id)
            {
                error.Err = "Nie jesteś wychowawcą klasy";
                error.Desc = "Nie możesz usunąć członka klasy";
                return StatusCode(409, error);
            }
            var userToDelete = await _apiHelper.ReturnUserByID(deleteMemberDTO.userToDeleteID);
            var deleteMember = await _apiHelper.DeleteMemberFromClass(userToDelete, classObj);
            return Ok(deleteMember);
        }

        [HttpPut("deleteSubject")]
        public async Task<IActionResult> DeleteSubject([FromBody]DeleteSubjectDTO deleteSubject)
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
            var classObj = await _apiHelper.ReturnClassByID(deleteSubject.classID);
            if(classObj == null)
            {
                error.Err = "Zle ID klasy";
                error.Desc = "Wprowadz ID klasy ponownie";
                return StatusCode(409, error);
            }
            if(classObj.creatorID != id)
            {
                error.Err = "Nie jesteś wychowawcą klasy";
                error.Desc = "Nie możesz usunąć przedmiotu";
                return StatusCode(409, error);
            }
            var isDeleted = await _apiHelper.IsSubjectDeleted(deleteSubject.classID, deleteSubject.subjectID, id);
            if(isDeleted)
            {
                error.Err = "Pomyślnie usunięto przedmiot";
                error.Desc = "Udało się usunąć przedmiot";
                return StatusCode(200, error);
            }
            else
            {
                error.Err = "Zle ID przedmiotu";
                error.Desc = "Wprowadz poprawne ID przedmiotu";
                return StatusCode(409, error);
            }

        }


    }
}