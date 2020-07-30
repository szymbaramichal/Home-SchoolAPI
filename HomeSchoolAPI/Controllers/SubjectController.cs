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
    public class SubjectController : ControllerBase
    {
        private Error error;
        private readonly ITokenHelper _tokenHelper;
        private readonly IApiHelper _apiHelper;
        public SubjectController(ITokenHelper tokenHelper, IApiHelper apiHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }

        /// <summary>
        /// Creating subject in class.
        /// </summary>
        [HttpPost("create")]
        [TokenAuthorization]
        public async Task<IActionResult> CreateSubject(CreateSubjectDTO createSubjectDTO)
        {

            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);

            var user = await _apiHelper.ReturnUserByMail(createSubjectDTO.UserToAddEmail);

            if(user == null || user.userRole == 0)
            {
                error.Err = "Nie poprawny nauczyciel";
                error.Desc = "Wprowadz email nauczyciela jeszcze raz";
                return StatusCode(405, error);
            }

            var classObj = await _apiHelper.ReturnClassByID(createSubjectDTO.ClassID);

            if(classObj == null)
            {
                error.Err = "Nie ma takiej klasy";
                error.Desc = "Wprowadź ID klasy jeszcze raz";
                return StatusCode(405, error);
            }

            if(classObj.creatorID != id)
            {
                error.Err = "Nie jestes wychowawcą";
                error.Desc = "Nie możesz tworzyć przedmiotu";
                return StatusCode(405, error);
            }

            var subject = await _apiHelper.AddSubjectToClass(user.Id, classObj, createSubjectDTO.SubjectName);
            
            await _apiHelper.ReplaceClassInfo(subject.classObj);

            return Ok(subject);
        }

        /// <summary>
        /// Deleting subject in class.
        /// </summary>
        [HttpPut("deleteSubject")]
        [TokenAuthorization]
        public async Task<IActionResult> DeleteSubject([FromBody]DeleteSubjectDTO deleteSubject)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(deleteSubject.ClassID);
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
            var isDeleted = await _apiHelper.IsSubjectDeleted(deleteSubject.ClassID, deleteSubject.SubjectID, id);
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