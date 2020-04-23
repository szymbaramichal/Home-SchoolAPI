using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Models;
using MongoDB.Driver;

namespace HomeSchoolAPI.Helpers
{
    public class ApiHelper : IApiHelper
    {
        private IMongoCollection<Class> _class;
        private IMongoCollection<User> _users;
        private IMongoCollection<Subject> _subjects;
        private IMongoDatabase database; 
        public ApiHelper()
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
        }

        #region UsersMethods
        public async Task<UserToReturn> ReturnUserToReturn(User user)
        {
            UserToReturn userToReturn = new UserToReturn();
            userToReturn.Id = user.Id;
            userToReturn.email = user.email;
            userToReturn.name = user.name;
            userToReturn.surrname = user.surrname;
            userToReturn.userRole = user.userRole;
            userToReturn.userCode = user.userCode;
            userToReturn.classes = new List<ClassToReturn>();
            var classes = user.classMember.ToArray();
            for (int i = 0; i < user.classMember.Count; i++)
            {
                var classObj = await ReturnClassByID(classes[i]);
                var classa = await ReturnClassToReturn(classObj);
                userToReturn.classes.Add(classa);
            }
            return userToReturn;
        }
        public async Task<User> ReturnUserByID(string id)
        {
            var user = await _users.Find<User>(user => user.Id == id).FirstOrDefaultAsync();
            return user;
        }
        public async Task<User> ReturnUserByMail(string email)
        {
            var user = await _users.Find<User>(user => user.email == email).FirstOrDefaultAsync();
            return user;
        }
        public bool DoesUserExistByEmail(string email)
        {
            return _users.Find<User>(user => user.email == email).Any();
        }
        #endregion 

        #region ClasessMethods
        public async Task<ClassToReturn> ReturnClassToReturn(Class classObj)
        {
            _subjects = database.GetCollection<Subject>(classObj.Id+"_su");
            ClassToReturn classToReturn = new ClassToReturn();
            List<Subject> subjects = new List<Subject>();
            classToReturn.Id = classObj.Id;
            classToReturn.creatorID = classObj.creatorID;
            classToReturn.className = classObj.className;
            classToReturn.schoolName = classObj.schoolName;
            classToReturn.membersAmount = classObj.membersAmount;
            classToReturn.members = classObj.members;
            classToReturn.subjects = subjects;
            var subjectsObj = classObj.subjects.ToArray();
            for (int i = 0; i < classObj.subjects.Count; i++)
            {
                var subjectObj = await _subjects.Find<Subject>(x => x.Id == subjectsObj[i]).FirstOrDefaultAsync();
                classToReturn.subjects.Add(subjectObj);
            }
            return classToReturn;
        }

        public async Task<Class> ReturnClassByID(string id)
        {
            try
            {
                _class = database.GetCollection<Class>(id);
                var klasa = await _class.Find<Class>(x => x.Id == id).FirstOrDefaultAsync();
                if(klasa == null)
                {
                    return null;
                }
                return klasa;
            }
            catch
            {
                return null;
            }
        }

        public bool IsUserInClass(string userID, Class classa)
        {
            for (int i = 0; i < classa.members.Count; i++)
            {
                if(classa.members.Contains(userID))
                {
                    return true;
                }
            }
            return false;
        }



        public async Task<List<Class>> ReturnAllClasses(string userId)
        {
            var user = await _users.Find<User>(x => x.Id == userId).FirstOrDefaultAsync();
            if(user == null) return null;
            List<Class> klasa = new List<Class>();
            for (int i = 0; i < user.classMember.Count; i++)
            {
                _class = database.GetCollection<Class>(user.classMember[i]);
                var toAdd = await _class.Find<Class>(x => x.Id == user.classMember[i]).FirstOrDefaultAsync();
                klasa.Add(toAdd);
            }
            return klasa;
        }


        public async Task<Class> AddMemberToClass(string email, Class classe)
        {
            _class = database.GetCollection<Class>(classe.Id);
            _class.Find<Class>(x => x.Id == classe.Id);
            var filter = Builders<Class>.Filter.Eq(c => c.Id, classe.Id);
            var user = await _users.Find<User>(user => user.email == email).FirstOrDefaultAsync();
            for (int i = 0; i < classe.members.Count; i++)
            {
                if(classe.members.Contains(user.Id))
                {
                    return null;
                }
            }
            classe.members.Add(user.Id);
            classe.membersAmount++;
            await _class.ReplaceOneAsync(filter, classe);
            return classe;
        }

        public async Task<Class> CreateClass(User creator, string className, string schoolName)
        {
            List<string> members = new List<string>();
            List<string> subjects = new List<string>();
            await database.CreateCollectionAsync(className);

            await database.CreateCollectionAsync(className+"_su");
            await database.CreateCollectionAsync(className+"_ho");

            _class = database.GetCollection<Class>(className);
            Class classToAdd = new Class 
            {
                className = className,
                creatorID = creator.Id,
                schoolName = schoolName,
                membersAmount = 0,
                members = members,
                subjects = subjects
            };
            classToAdd.membersAmount++;
            classToAdd.members.Add(creator.Id);
                
            await _class.InsertOneAsync(classToAdd);
            var classObj = await _class.Find<Class>(x => x.className == className).FirstOrDefaultAsync();
            await database.RenameCollectionAsync(className, classObj.Id);
            await database.RenameCollectionAsync(className+"_su", classObj.Id+"_su");
            await database.RenameCollectionAsync(className+"_ho", classObj.Id+"_ho");
            var filter = Builders<User>.Filter.Eq(u => u.Id, creator.Id);
            creator.classMember.Add(classObj.Id);
            await _users.ReplaceOneAsync(filter, creator);
                    
            return classObj;
        }

        public async Task<Class> ReplaceClassInfo(Class classToChange)
        {
            _class = database.GetCollection<Class>(classToChange.Id);
            var filter = Builders<Class>.Filter.Eq(x => x.Id, classToChange.Id);
            await _class.ReplaceOneAsync(filter, classToChange);
            return classToChange;
        }
        #endregion

        #region SubjectsMethods
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
            
            var isTeacherAlreadyInClass = false;
            for (int i = 0; i < classToEdit.members.Count; i++)
            {
                if(classToEdit.members.Contains(teacherId))
                {
                    isTeacherAlreadyInClass = true;
                }
            }
            
            if(!isTeacherAlreadyInClass)
            {
                classToEdit.membersAmount++;
                classToEdit.members.Add(teacherId);
            }
            
            SubjectReturn subjectReturn = new SubjectReturn();
            subjectReturn.classObj = classToEdit;
            subjectReturn.subject = subjectDocument;
            return subjectReturn;

        }


        public async Task<List<Subject>> ReturnAllSubjects(List<Class> userClases)
        {
            List<Subject> subjects = new List<Subject>();
            var klasy = userClases.ToArray();

            for (int i = 0; i < userClases.Count; i++)
            {
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

        #endregion
    }
}