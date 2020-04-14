using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Models;
using MongoDB.Driver;

namespace HomeSchoolAPI.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IMongoCollection<User> _users;
        public UserHelper()
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            var database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
        }
        public UserToReturn ReturnUserToReturn(User userFromRepo)
        {
            UserToReturn userToReturn = new UserToReturn();
            userToReturn.Id = userFromRepo.Id;
            userToReturn.email = userFromRepo.email;
            userToReturn.name = userFromRepo.name;
            userToReturn.surrname = userFromRepo.surrname;
            userToReturn.userRole = userFromRepo.userRole;
            userToReturn.friends = userFromRepo.friends;
            userToReturn.userCode = userFromRepo.userCode;
            userToReturn.classMember = userFromRepo.classMember;

            return userToReturn;
        }

        public async Task<User> AddFriend(string userToAddID, User user)
        {

            for (int i = 0; i < user.friends.Count; i++)
            {
                if(user.friends[i].Contains(userToAddID))
                {
                    return null;
                }    
            }

            user.friends.Add(userToAddID);

            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            await _users.ReplaceOneAsync(filter, user);
            return user;

        }

        public async Task<User> ReturnUserByID(string id)
        {
            var user = await _users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();
            return user;
        }

        public bool DoesUserExist(string id)
        {
            return _users.Find<User>(user => user.Id == id).Any();
        }
    }
}