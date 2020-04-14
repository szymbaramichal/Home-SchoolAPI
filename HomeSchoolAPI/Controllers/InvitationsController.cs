using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.DTOs;
using HomeSchoolAPI.Helpers;
using HomeSchoolAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HomeSchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserHelper _userHelper;
        private String token;
        private Error error;
        
        public InvitationsController(ITokenHelper tokenHelper, IUserHelper userHelper)
        {
            error = new Error();
            _tokenHelper = tokenHelper;
            _userHelper = userHelper;
        }



        [HttpPut]
        public async Task<IActionResult> AddFriend([FromBody]UserToAddDTO userToAddID)
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

                //pobieram usera do którego dodaje znajomych
                var user = await _userHelper.ReturnUserByID(id);

                if(userToAddID.UserToAddID.Length != 24)
                {
                    error.Err = "Nieprawidlowe ID uzytkownika";
                    error.Desc = "ID ma dlugosc 24 znakow";
                    return StatusCode(405, error);
                }

                try
                {
                    var isValidInput = _userHelper.DoesUserExist(userToAddID.UserToAddID);
                    if(!isValidInput)
                    {
                        error.Err = "Nieprawidlowe ID uzytkownika";
                        error.Desc = "Wprowadz ID jeszcze raz";
                        return StatusCode(405, error);
                    }

                        var isNotAlreadyFriend = await _userHelper.AddFriend(userToAddID.UserToAddID, user);

                    if(isNotAlreadyFriend == null)
                    {
                        error.Err = "Już jesteście znajomymi";
                        error.Desc = "Już jesteście znajomymi, nie musisz zapraszać tego użytkownika";
                        return StatusCode(409, error);
                    }
                
                    return Ok(_userHelper.ReturnUserToReturn(user));
                }
                catch (System.Exception)
                {
                    error.Err = "Nieprawidlowe ID uzytkownika";
                    error.Desc = "Wprowadz ID jeszcze raz";
                    return StatusCode(405, error);
                }

        }

    }
}