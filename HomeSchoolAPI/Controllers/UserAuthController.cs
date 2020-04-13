using System;
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
        private Error error;
        private String token;
        public UserAuthController(IAuthRepo repo, IConfiguration configuration, IUserHelper userHelper, ITokenHelper tokenHelper)
        {
            _tokenHelper = tokenHelper;
            _userHelper = userHelper;
            error = new Error();
            _configuration = configuration;
            _repo = repo;
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
            if(userForRegister.Role == 0 && String.IsNullOrWhiteSpace(userForRegister.UserCode))
            {
                error.Err = "Błędny kod od nauczyciela";
                error.Desc = "Prosze wprowadzić poprawny kod od nauczyciela";
                return StatusCode(405, error);
            }
            if(userForRegister.Role != 1 && userForRegister.Role != 0)
            {
                error.Err = "Zła rola użytkownika";
                error.Desc = "Wprowadź rolę 1 lub 2";
                return StatusCode(405, error);
            }

            userForRegister.Email = userForRegister.Email.ToLower();
            
            if(await _repo.UserExists(userForRegister.Email))
            {
                error.Err = "Ten adres email jest już zajęty";
                error.Desc = "Wprowadź inny adres email";
                return StatusCode(405, error);
            }

            var userToCreate = new User 
            {
                email = userForRegister.Email,
                userRole = userForRegister.Role
                //ADD VALIDATION FOR USERCODE
            };

            var createdUser = await _repo.RegisterUser(userToCreate, userForRegister.Password);

            return StatusCode(201);
        }

        [HttpGet("loginviatoken")]
        public async Task<IActionResult> LoginViaToken()
        {
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
            var user = await _userHelper.ReturnUserByID(id);

            if (user == null)
            {
                error.Err = "Nie znaleziono takiego uzytkownika";
                error.Desc = "Wprowadz dane jeszcze raz";
                return StatusCode(405, error);
            }

            return Ok(_userHelper.ReturnUserToReturn(user));
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
                Expires = DateTime.Now,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            UserToReturn user = _userHelper.ReturnUserToReturn(userFromRepo);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                user
            });

        }
    }
}