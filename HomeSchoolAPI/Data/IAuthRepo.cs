using System.Threading.Tasks;
using HomeSchoolAPI.Models;

namespace HomeSchoolAPI.Data
{
    public interface IAuthRepo
    {
         Task<User> RegisterUser(User user, string password);
         Task<User> LoginUser(string email, string password);
         Task<bool> UserExists(string email);
    }
}