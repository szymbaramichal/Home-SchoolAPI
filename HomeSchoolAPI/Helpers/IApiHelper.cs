using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeSchoolAPI.Helpers
{
    public interface IApiHelper
    {
        #region Class
        Task<Class> ReturnClassByID(string id);
        Task<Class> CreateClass(User creator, string className, string schoolName);
        Task<Class> ReplaceClassInfo(Class classToChange);
        Task<Class> AddMemberToClass(string email, Class classObj);
        Task<ClassToReturn> ReturnClassToReturn(Class classObj, string userID);
        Task<SubjectReturn> AddSubjectToClass(string teacherId, Class classToEdit, string subjectName);
        #endregion
        #region Subject
        Task<Subject> ReturnSubjectByTeacherID(string classID,string id);
        Task<SubjectToReturn> ReturnSubjectToReturn(Subject subject, string userID);
        #endregion
        #region User
        UserToReturn ReturnUserToReturn(User user);
        Task<User> ReturnUserByID(string id);
        bool DoesUserExistByEmail(string email);
        Task<User> ReturnUserByMail(string email);
        #endregion
        #region Homework
        Task<HomeworkToReturn> ReturnHomeworkToReturn(Homework homework, string classID, string userID);
        Task<Homework> AddHomeworkToSubject(Subject subject, string name, string description, DateTime time);
        Task<Homework> UploadFileToHomework(IFormFile file, string classID, Homework homework, string senderID);
        Task<Homework> ReturnHomeworkByIDs(string classID, string homeworkID);
        Task<FileStreamResult> ReturnFileBySenderID(string homeworkID, string fileID);
        #endregion
        #region Response
        Task<Homework> CreateResponse(Response response, string classID);
        Task<Response> PutMark(string homeworkID, string responseID, string mark);
        #endregion
    }

}