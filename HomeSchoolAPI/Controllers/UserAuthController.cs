using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Data;
using HomeSchoolAPI.DTOs;
using HomeSchoolAPI.Helpers;
using HomeSchoolAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HomeSchoolAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        
        private readonly IConfiguration _configuration;
        private readonly IAuthRepo _repo;
        private readonly IUserHelper _userHelper;
        private readonly ITokenHelper _tokenHelper;
        private readonly IClassHelper _classHelper;
        private readonly ISubjectHelper _subjectHelper;
        private Error error;
        private String token;
        public UserAuthController(IAuthRepo repo, IConfiguration configuration, IUserHelper userHelper, ITokenHelper tokenHelper, IClassHelper classHelper, ISubjectHelper subjectHelper)
        {
            _classHelper = classHelper;
            _tokenHelper = tokenHelper;
            _userHelper = userHelper;
            error = new Error();
            _configuration = configuration;
            _repo = repo;
            _subjectHelper = subjectHelper;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegister)
        {
            if(String.IsNullOrWhiteSpace(userForRegister.Email) || String.IsNullOrWhiteSpace(userForRegister.Password))
            {
                error.Err = "Wszystkie pola muszą być wypełnione";
                error.Desc = "Wprowadź wszystkie dane";
                return StatusCode(405, error);
            }

            List<string> list1 = new List<string>();
            
            var userToCreate = new User 
            {
                email = userForRegister.Email,
                userRole = userForRegister.UserRole,
                classMember = list1,
                name = "test",
                surrname = "test",
                pendingInvitations = list1
            };

            var isUserExisting = await _repo.UserExists(userForRegister.Email);
            
            if(isUserExisting)
            {
                error.Err = "Ten adres email jest już zajęty";
                error.Desc = "Wprowadź inny adres email";
                return StatusCode(405, error);
            }

            if(userForRegister.UserRole != 1 && userForRegister.UserRole != 0)
            {
                error.Err = "Zła rola użytkownika";
                error.Desc = "Wprowadź rolę 1 lub 0";
                return StatusCode(405, error);
            }

            if(userForRegister.UserRole == 0)
            {
                var classa = await _classHelper.ReturnClassByID(userForRegister.UserCode);

                if(classa == null)
                {
                    error.Err = "Błędny kod klasy";
                    error.Desc = "Prosze wprowadzić poprawny kod od nauczyciela";
                    return StatusCode(405, error);
                }
                userToCreate.classMember.Add(classa.Id);
                userToCreate.userCode = classa.Id;
            }

            if(userForRegister.UserRole == 1)
            {
                userToCreate.userCode = null;
            }

            userForRegister.Email = userForRegister.Email.ToLower();

            var createdUser = await _repo.RegisterUser(userToCreate, userForRegister.Password);

            if(userForRegister.UserRole == 0)
            {
                var classa = await _classHelper.ReturnClassByID(userForRegister.UserCode);
                classa.membersAmount++;
                var userr = await _userHelper.ReturnUserByMail(userForRegister.Email);
                classa.members.Add(userr.Id);
                await _classHelper.ReplaceClassInfo(classa);
            }
            var user = _userHelper.ReturnUserToReturn(userToCreate);

            return StatusCode(201, user);
        }

        [HttpGet("loginviatoken")]
        public async Task<IActionResult> LoginViaToken()
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
            var user = await _userHelper.ReturnUserByID(id);

            if (user == null)
            {
                error.Err = "Nie znaleziono takiego uzytkownika";
                error.Desc = "Wprowadz dane jeszcze raz";
                return StatusCode(405, error);
            }

            var userToReturn = _userHelper.ReturnUserToReturn(user);
            var userClasses = await _classHelper.ReturnAllClasses(id);
            var classSubjects = await _subjectHelper.ReturnAllSubjects(userClasses);
            return Ok(new {
                userToReturn,
                userClasses,
                classSubjects
            });
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLogin)
        {
            if(String.IsNullOrEmpty(userForLogin.Email) || String.IsNullOrEmpty(userForLogin.Password))
            {
                error.Err = "Wszystkie pola muszą zostać wypełnione i nie mogą być puste!";
                error.Desc = "Uzupełnij wszystkie pola";
                return Unauthorized(error);
            }

            var userFromRepo = await _repo.LoginUser(userForLogin.Email.ToLower(), userForLogin.Password);

            if(userFromRepo == null) 
            {
                error.Err = "Niepoprawny email lub hasło";
                error.Desc = "Uzupełnij pola od nowa";
                return Unauthorized(error);
            }
        

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Email, userFromRepo.email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            UserToReturn userToReturn = _userHelper.ReturnUserToReturn(userFromRepo);

            var userClasses = await _classHelper.ReturnAllClasses(userFromRepo.Id);
            
            var classSubjects = await _subjectHelper.ReturnAllSubjects(userClasses);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                userToReturn,
                userClasses,
                classSubjects
            });

        }
    }
}