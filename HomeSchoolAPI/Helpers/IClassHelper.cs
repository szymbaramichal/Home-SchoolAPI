using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.Models;

namespace HomeSchoolAPI.Helpers
{
    public interface IClassHelper
    {
        Task<Class> ReturnClassByID(string id);
        Task<Class> CreateClass(User creator, string className, string schoolName);
        Task<List<Class>> ReturnAllClasses(string userId);
        Task<Class> ReplaceClassInfo(Class classToChange);
        Task<Class> AddMemberToClass(string email, Class classe);
        bool IsUserInClass(string userID, Class classa);
    }
}