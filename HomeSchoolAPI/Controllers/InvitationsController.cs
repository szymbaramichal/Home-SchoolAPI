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
        private readonly IMongoCollection<User> _users;

        private readonly ITokenHelper _tokenHelper;
        private readonly IUserHelper _userHelper;
        private String token;
        private Error error;
        
        public InvitationsController(ITokenHelper tokenHelper, IUserHelper userHelper)
        {
            error = new Error();
            _tokenHelper = tokenHelper;
            _userHelper = userHelper;
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            var database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
        }



        [HttpPost]
        public async Task<IActionResult> AddFriend(UserToAddDTO userToAddID)
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
                var isValidInput = _users.Find<User>(user => user.Id == userToAddID.UserToAddID).Any();
                try
                {
                    if(!isValidInput)
                    {
                        error.Err = "Nieprawidlowe ID uzytkownika";
                        error.Desc = "Wprowadz ID jeszcze raz";
                        return StatusCode(405, error);
                    }
                }
                catch
                {
                    error.Err = "Nieprawidlowe ID uzytkownika";
                    error.Desc = "Wprowadz ID jeszcze raz";
                    return StatusCode(405, error);
                }

                var addingFriend = await _userHelper.AddFriend(userToAddID.UserToAddID, user);

                if(addingFriend == null)
                {
                        error.Err = "Już jesteście znajomymi";
                        error.Desc = "Już jesteście znajomymi, nie musisz zapraszać tego użytkownika";
                        return StatusCode(405, error);
                }
                
                return Ok(_userHelper.ReturnUserToReturn(user));

        }

    }
}