using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Helpers;
using HomeSchoolAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HomeSchoolAPI.Controllers
{
    [Authorize]
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

        [HttpPut]
        public async Task<IActionResult> AddFriend(string[] userIDs)
        {

            try
            {
                token = HttpContext.Request.Headers["Authorization"];
                token = token.Replace("Bearer ", "");
            }
            catch
            {
                error.Err = "Nieprawidlowy token";
                error.Desc = "Wprowadz token jeszcze raz";
            }
                //validating token
                if(!_tokenHelper.IsValidateToken(token)) return Unauthorized("Token expired");

                var id = _tokenHelper.GetIdByToken(token);

                //pobieram usera do którego dodaje znajomych
                var user = _userHelper.ReturnUserByIDSync(id);

                List<string> friendsOfUser = new List<string>();
                friendsOfUser = user.friends;

                for (int i = 0; i < userIDs.Length; i++)
                {
                    friendsOfUser.Add(userIDs[i]);
                }

                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                var userToUpdate = user;
                userToUpdate.friends = friendsOfUser;

                await _users.ReplaceOneAsync(filter, userToUpdate);
                return Ok(_userHelper.ReturnUser(user));

        }

    }
}