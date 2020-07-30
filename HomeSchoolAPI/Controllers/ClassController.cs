using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Filters;
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
        private Error error;
        public ClassController(ITokenHelper tokenHelper, IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
            error = new Error();
            _tokenHelper = tokenHelper;
        }

        /// <summary>
        /// Creating class.
        /// </summary>
        [HttpPost("create")]
        [TokenAuthorization]
        public async Task<IActionResult> CreateClass(ClassToCreateDTO classToCreate)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            if(String.IsNullOrWhiteSpace(classToCreate.ClassName) || String.IsNullOrWhiteSpace(classToCreate.SchoolName))
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
                    var createdClass = await _apiHelper.CreateClass(creator, classToCreate.ClassName, classToCreate.SchoolName);
                    return Ok(createdClass);
                }
                else
                {
                    error.Err = "Nie jestes nauczycielem";
                    error.Desc = "Nie mozesz zalozyc klasy";
                    return StatusCode(409, error);
                }
        }
    
        /// <summary>
        /// Adding teacher or student to class.
        /// </summary>
        [HttpPut("addMember")]
        [TokenAuthorization]
        public async Task<IActionResult> AddMemberToClass([FromBody]AddToClassDTO addToClassDTO)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

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

        /// <summary>
        /// Deleting user or teacher from class.
        /// </summary>
        [HttpPut("deleteMember")]
        [TokenAuthorization]

        //TODO: Comment all methods in HomeSchoolAPI and other projects, check logic in Core, potem na trello popatrzeć
        public async Task<IActionResult> DeleteMember([FromBody]DeleteMemberDTO deleteMemberDTO)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(deleteMemberDTO.ClassID);
            if(classObj == null)
            {
                error.Err = "Niepoprawne ID klasy";
                error.Desc = "Nie możesz usunąć członka klasy";
                return StatusCode(409, error);
            }
            if(!classObj.members.Contains(deleteMemberDTO.UserToDeleteID))
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
            var userToDelete = await _apiHelper.ReturnUserByID(deleteMemberDTO.UserToDeleteID);
            var deleteMember = await _apiHelper.DeleteMemberFromClass(userToDelete, classObj);
            return Ok(deleteMember);
        }
    }
}