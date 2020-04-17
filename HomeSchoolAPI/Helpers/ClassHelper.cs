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
        public ClassHelper()
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
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


        public async Task<Class> CreateClass(User creator, string className, string schoolName)
        {
            List<string> list1 = new List<string>();
            await database.CreateCollectionAsync(className);

            _class = database.GetCollection<Class>(className);
            Class classToAdd = new Class 
            {
                className = className,
                creatorID = creator.Id,
                schoolName = schoolName,
                membersAmount = 0,
                members = list1,
                subjects = list1
            };
                
            await _class.InsertOneAsync(classToAdd);
            var klasa = await _class.Find<Class>(x => x.className == className).FirstOrDefaultAsync();
            await database.RenameCollectionAsync(className, klasa.Id);
            var filter = Builders<User>.Filter.Eq(u => u.Id, creator.Id);
            creator.classMember.Add(klasa.Id);
            await _users.ReplaceOneAsync(filter, creator);
                    
            return klasa;
        }
    }
}