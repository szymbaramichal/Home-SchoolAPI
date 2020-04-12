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
        public UserToReturn ReturnUser(User userFromRepo)
        {
            UserToReturn userToReturn = new UserToReturn();
            userToReturn.Id = userFromRepo.Id;
            userToReturn.email = userFromRepo.email;
            userToReturn.name = userFromRepo.name;
            userToReturn.surrname = userFromRepo.surrname;
            userToReturn.userRole = userFromRepo.userRole;
            userToReturn.friends = userFromRepo.friends;
            userToReturn.username = userFromRepo.username;
            userToReturn.userCode = userFromRepo.userCode;

            return userToReturn;
        }

        public async Task<User> ReturnUserByID(string id)
        {
            var user = await _users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();
            return user;
        }
        public User ReturnUserByIDSync(string id)
        {
            var user = _users.Find<User>(user => user.Id == id).FirstOrDefault();
            return user;
        }

    }
}