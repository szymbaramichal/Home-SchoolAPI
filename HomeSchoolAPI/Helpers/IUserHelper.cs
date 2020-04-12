using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Models;

namespace HomeSchoolAPI.Helpers
{
    public interface IUserHelper
    {
        UserToReturn ReturnUser(User userFromRepo);
        Task<User> ReturnUserByID(string id);
        User ReturnUserByIDSync(string id);
    }
}