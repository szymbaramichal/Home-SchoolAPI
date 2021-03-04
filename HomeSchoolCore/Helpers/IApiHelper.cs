using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Models;
using Microsoft.AspNetCore.Http;

namespace HomeSchoolCore.Helpers
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
        Task<Class> DeleteMemberFromClass(User user, Class classObj);
        Task<List<string>> ReturnNames(Class classObj);
        Task<bool> DoesUserBelongToClass(string userId, string classId);
        #endregion
        #region Subject
        Task<SubjectToReturn> ReturnSubjectToReturn(Subject subject, string userID);
        Task<Subject> ReturnSubjectBySubjectID(string classID, string subjectID);
        Task<bool> IsSubjectDeleted(string classID, string subjectID, string userID);
        #endregion
        #region User
        Task<User> RegisterUser(User user, string password);
        Task<User> LoginUser(string email, string password);
        Task<bool> UserExists(string email);
        UserToReturn ReturnUserToReturn(User user);
        Task<User> ReturnUserByID(string id);
        bool DoesUserExistByEmail(string email);
        Task<User> ReturnUserByMail(string email);
        #endregion
        #region Homework
        Task<HomeworkToReturn> ReturnHomeworkToReturn(Homework homework, string classID, string userID);
        Task<Homework> AddHomeworkToSubject(Subject subject, string name, string description, DateTime time, List<string> filesID, List<string> linkHrefs);
        Task<string> UploadFileToHomework(IFormFile file, string classID, string senderID, string subjectID);
        Task<Homework> ReturnHomeworkByIDs(string classID, string homeworkID);
        Task<ReturnFile> ReturnHomeworkFileBySenderID(string homeworkID, string fileID);
        Task<bool> IsHomeworkDeleted(string homeworkID, string subjectID, string classID);
        #endregion
        #region Response
        Task<ResponseReturn> CreateResponse(ResponseToHomework response, string classID, Homework homework);
        Task<ResponseToHomework> PutMark(string homeworkID, string responseID, string mark);
        Task<string> UploadFileToResponse(IFormFile file, string homeworkID, string senderID, string subjectID, string classID);
        Task<ReturnFile> ReturnResponseFileBySenderID(string homeworkID, string fileID);

        #endregion
        #region TextMessages
        Task<List<TextMessage>> ReturnLastMessages(string subjectID);
        Task<TextMessage> SendMessage(string subjectID, TextMessage textMessage);
        Task<List<TextMessage>> ReturnNewerMessages(int lastMessageID, string subjectID);
        Task<List<TextMessage>> ReturnOlderMessages(int lastMessageID, string subjectID);
        #endregion
        #region Quizes
        Task<Quiz> IsQuizAdded(CreateQuizDTO quizObj);
        Task<QuizesToReturn> ReturnQuizesForSubject(string classId, string subjectId, string userId);
        Task<Quiz> ReturnQuizById(string classId, string quizId);
        Task<bool> SaveAnswersToQuiz(ResponseToQuiz responseToQuiz);
        Task<List<QuestionToReturn>> ReturnQuestionsForQuiz(string classId, string quizId);
        Task<QuizQuestion> ReutrnCorrectQuizQuestions(string classId, string quizId);
        Task<List<AnswerToReturn>> GetAnswersForStudent(string id);
        #endregion
    }

}