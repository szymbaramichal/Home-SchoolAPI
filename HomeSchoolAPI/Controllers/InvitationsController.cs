using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Helpers;
using HomeSchoolAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HomeSchoolAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        
        public InvitationsController()
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            var database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
        }

        [HttpPut]
        public async Task<IActionResult> AddFriend(string[] userIDs)
        {
                //TODO: Middleware do pobierania i walidacji tokenu
                //getting token from header
                String token = HttpContext.Request.Headers["Authorization"];
                token = token.Replace("Bearer ", "");

                //validating token
                TokenHelper helper = new TokenHelper();
                UserHelper userHelper = new UserHelper();
                if(!helper.IsValidateToken(token)) return Unauthorized("Token expired");

                var id = helper.GetIdByToken(token);

                //pobieram usera do którego dodaje znajomych
                var user = await userHelper.ReturnUserByID(id);

                List<string> friendsOfUser = new List<string>();

                for (int i = 0; i < userIDs.Length; i++)
                {
                    friendsOfUser.Add(userIDs[i]);
                }

                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                var update = Builders<User>.Update.Set(u => u.friends, friendsOfUser);
                _users.UpdateOne(filter, update);

                return Ok(user);

        }

    }
}