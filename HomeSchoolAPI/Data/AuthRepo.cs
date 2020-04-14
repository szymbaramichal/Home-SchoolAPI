using System;
using System.Threading.Tasks;
using HomeSchoolAPI.Models;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace HomeSchoolAPI.Data
{
    public class AuthRepo : IAuthRepo
    {
        private readonly IMongoCollection<User> _users;

        public AuthRepo()
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            var database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
        }
        public async Task<User> LoginUser(string email, string password)
        {
            var user = await _users.Find<User>(user => user.email == email).FirstOrDefaultAsync();
            if(user == null) return null;

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> RegisterUser(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            //add pendingInvitations and checking if user is student than userCode is required

            await _users.InsertOneAsync(user);
            return user;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string email)
        {
            if(await _users.Find<User>(user => user.email == email).AnyAsync()) return true;
            return false;
        }
    }
}