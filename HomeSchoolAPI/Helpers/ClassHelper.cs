using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.Models;
using MongoDB.Driver;

namespace HomeSchoolAPI.Helpers
{
    public class ClassHelper : IClassHelper
    {
        private IMongoCollection<Class> _class;
        private IMongoCollection<User> _users;
        private IMongoDatabase database;
        private readonly IUserHelper _userHelper;
        public ClassHelper(IUserHelper userHelper)
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
            _userHelper = userHelper;
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
            var user = await _userHelper.ReturnUserByMail(email);
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
            var klasa = await _class.Find<Class>(x => x.className == className).FirstOrDefaultAsync();
            await database.RenameCollectionAsync(className, klasa.Id);
            await database.RenameCollectionAsync(className+"_su", klasa.Id+"_su");
            var filter = Builders<User>.Filter.Eq(u => u.Id, creator.Id);
            creator.classMember.Add(klasa.Id);
            await _users.ReplaceOneAsync(filter, creator);
                    
            return klasa;
        }

        public async Task<Class> ReplaceClassInfo(Class classToChange)
        {
            _class = database.GetCollection<Class>(classToChange.Id);
            var filter = Builders<Class>.Filter.Eq(x => x.Id, classToChange.Id);
            await _class.ReplaceOneAsync(filter, classToChange);
            return classToChange;
        }
    }
}