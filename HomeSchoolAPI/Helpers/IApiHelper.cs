using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Models;
using Microsoft.AspNetCore.Http;

namespace HomeSchoolAPI.Helpers
{
    public interface IApiHelper
    {
        Task<Class> ReturnClassByID(string id);
        Task<Class> CreateClass(User creator, string className, string schoolName);
        Task<List<Class>> ReturnAllClasses(string userId);
        Task<Class> ReplaceClassInfo(Class classToChange);
        Task<Class> AddMemberToClass(string email, Class classe);
        Task<ClassToReturn> ReturnClassToReturn(Class classObj, string userID);
        Task<SubjectReturn> AddSubjectToClass(string teacherId, Class classToEdit, string subjectName);
        Task<List<Subject>> ReturnAllSubjects(List<Class> userClases);
        UserToReturn ReturnUserToReturn(User user);
        Task<User> ReturnUserByID(string id);
        bool DoesUserExistByEmail(string email);
        Task<User> ReturnUserByMail(string email);
        Task<Homework> AddHomeworkToSubject(Subject subject, string name, string description);
        Task<Subject> ReturnSubjectByTeacherID(string classID,string id);
        Task<SubjectToReturn> ReturnSubjectToReturn(Subject subject);
    }

}