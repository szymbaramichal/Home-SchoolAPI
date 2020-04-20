using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Models;
using MongoDB.Driver;

namespace HomeSchoolAPI.Helpers
{
    public class SubjectHelper : ISubjectHelper
    {        
        private IMongoCollection<User> _users;
        private IMongoCollection<Subject> _subjects;
        private IMongoDatabase database;
        private readonly IUserHelper _userHelper;
        private readonly IClassHelper _classHelper;
        public SubjectHelper(IUserHelper userHelper, IClassHelper classHelper)
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
            _userHelper = userHelper;
            _classHelper = classHelper;
        }
        public async Task<SubjectReturn> AddSubjectToClass(string teacherId, Class classToEdit, string subjectName)
        {
            try
            {
                await database.CreateCollectionAsync(classToEdit.Id+"_su_"+subjectName);
            }
            catch
            {
                return null;
            }
            _subjects = database.GetCollection<Subject>(classToEdit.Id+"_su_"+subjectName);

            Subject subject = new Subject();
            subject.name = subjectName;
            subject.teacherId = teacherId;
            subject.classID = classToEdit.Id;
            subject.homeworks = new List<string>();

            await _subjects.InsertOneAsync(subject);

            var subjectDocument = await _subjects.Find<Subject>(x => x.name == subjectName).FirstOrDefaultAsync();

            
            classToEdit.subjects.Add(subjectDocument.Id);

            var isTeacherAlreadyInClass = _classHelper.IsUserInClass(teacherId,classToEdit);
            if(!isTeacherAlreadyInClass)
            {
                classToEdit.membersAmount++;
                classToEdit.members.Add(teacherId);
            }

            await _classHelper.ReplaceClassInfo(classToEdit);
            SubjectReturn subjectReturn = new SubjectReturn();
            subjectReturn.classObj = classToEdit;
            subjectReturn.subject = subjectDocument;
            return subjectReturn;

        }
    }
}