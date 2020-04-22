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
        private IMongoCollection<Class> _classes;
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
            _subjects = database.GetCollection<Subject>(classToEdit.Id+"_su");        
            
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

        public async Task<List<Subject>> ReturnAllSubjects(List<Class> userClases)
        {
            List<Subject> subjects = new List<Subject>();

            for (int i = 0; i < userClases.Count; i++)
            {
                var klasy = userClases.ToArray();
                var klasa = klasy[i];

                for (int j = 0; j < klasa.subjects.Count; j++)
                {
                    var subjectss = klasa.subjects.ToArray();
                    var subject = subjectss[j];
                    _subjects = database.GetCollection<Subject>(klasa.Id+"_su");
                    
                    var temat = await _subjects.Find<Subject>(x => x.Id == subject).FirstOrDefaultAsync();
                    subjects.Add(temat);
                }
            } 
            return subjects;
        }

    }
}