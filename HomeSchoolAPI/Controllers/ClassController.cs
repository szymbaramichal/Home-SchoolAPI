using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Data;
using HomeSchoolAPI.DTOs;
using HomeSchoolAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HomeSchoolAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private IMongoCollection<Class> _class;
        private IMongoCollection<User> _users;
        private IMongoDatabase database;
        private Error error;
        public ClassController()
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            database = client.GetDatabase("ELearningDB");
            error = new Error();
            _users = database.GetCollection<User>("Users");
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass(ClassToCreateDTO classToCreate)
        {
            _class = database.GetCollection<Class>(classToCreate.className);
            List<string> list1 = new List<string>();

            try
            {
                var creator = await _users.Find(x => x.Id == classToCreate.creatorID).FirstOrDefaultAsync();

                if(creator.userRole == 1)
                {
                    try
                    {
                        await database.CreateCollectionAsync(classToCreate.className);
                    }
                    catch
                    {
                        error.Err = "Już istnieje taka klasa";
                        error.Desc = "Proszę wybrać inną nazwę klasy";
                        return StatusCode(409, error);
                    }
                    Class classToAdd = new Class 
                    {
                        className = classToCreate.className,
                        creatorID = classToCreate.creatorID,
                        schoolName = classToCreate.schoolName,
                        membersAmount = 0,
                        members = list1,
                        subjects = list1,
                    };
                    await _class.InsertOneAsync(classToAdd);
                    return Ok(classToAdd);
                }
                else
                {
                    error.Err = "Nie jestes nauczycielem";
                    error.Desc = "Nie mozesz zalozyc klasy";
                    return StatusCode(409, error);
                }
            }
            catch
            {
                error.Err = "Nieprawidłowe ID nauczyciela";
                error.Desc = "Wprowadz poprawny format ID nauczyciela";
                return StatusCode(409, error);
            }
        }

    }
}