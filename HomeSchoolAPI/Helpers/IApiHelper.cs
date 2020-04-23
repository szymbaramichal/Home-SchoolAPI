using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Models;

namespace HomeSchoolAPI.Helpers
{
    public interface IApiHelper
    {
        Task<Class> ReturnClassByID(string id);
        Task<Class> CreateClass(User creator, string className, string schoolName);
        Task<List<Class>> ReturnAllClasses(string userId);
        Task<Class> ReplaceClassInfo(Class classToChange);
        Task<Class> AddMemberToClass(string email, Class classe);
        Task<ClassToReturn> ReturnClassToReturn(Class classObj);
        Task<SubjectReturn> AddSubjectToClass(string teacherId, Class classToEdit, string subjectName);
        Task<List<Subject>> ReturnAllSubjects(List<Class> userClases);
        Task<UserToReturn> ReturnUserToReturn(User userFromRepo);
        Task<User> ReturnUserByID(string id);
        bool DoesUserExistByEmail(string email);
        Task<User> ReturnUserByMail(string email);
    }

}