using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Data;
using HomeSchoolAPI.DTOs;
using HomeSchoolAPI.Helpers;
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
        private IUserHelper _userHelper;
        private readonly ITokenHelper _tokenHelper;
        private String token;
        private Error error;
        public ClassController(ITokenHelper tokenHelper, IUserHelper userHelper)
        {
            var client = new MongoClient("mongodb+srv://majkii2115:Kruku2115@homeschool-ruok3.mongodb.net/test?retryWrites=true&w=majority");
            database = client.GetDatabase("ELearningDB");
            _users = database.GetCollection<User>("Users");
            error = new Error();
            _tokenHelper = tokenHelper;
            _userHelper = userHelper;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateClass(ClassToCreateDTO classToCreate)
        {
            try
            {
                token = HttpContext.Request.Headers["Authorization"];
                token = token.Replace("Bearer ", "");
                if (!_tokenHelper.IsValidateToken(token))
                {
                    error.Err = "Token wygasł";
                    error.Desc = "Zaloguj się od nowa";
                    return StatusCode(405, error);
                }
            }
            catch
            {
                error.Err = "Nieprawidlowy token";
                error.Desc = "Wprowadz token jeszcze raz";
                return StatusCode(405, error);
            }            

                List<string> list1 = new List<string>();
                var id = _tokenHelper.GetIdByToken(token);
                var creator = await _userHelper.ReturnUserByID(id);

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
                    _class = database.GetCollection<Class>(classToCreate.className);
                    Class classToAdd = new Class 
                    {
                        className = classToCreate.className,
                        creatorID = creator.Id,
                        schoolName = classToCreate.schoolName,
                        membersAmount = 0,
                        members = list1,
                        subjects = list1,
                    };
                    
                    await _class.InsertOneAsync(classToAdd);

                    var klasa = await _class.Find<Class>(x => x.className == classToCreate.className).FirstOrDefaultAsync();
                    var filter = Builders<User>.Filter.Eq(u => u.Id, creator.Id);
                    creator.classMember.Add(klasa.className);
                    await _users.ReplaceOneAsync(filter, creator);
                    
                    return Ok(classToAdd);
                }
                else
                {
                    error.Err = "Nie jestes nauczycielem";
                    error.Desc = "Nie mozesz zalozyc klasy";
                    return StatusCode(409, error);
                }
        }
    
        [HttpPut("add")]
        public async Task<IActionResult> AddMemberToClass([FromBody]AddToClassDTO addToClassDTO)
        {
            try
            {
                token = HttpContext.Request.Headers["Authorization"];
                token = token.Replace("Bearer ", "");
                if (!_tokenHelper.IsValidateToken(token))
                {
                    error.Err = "Token wygasł";
                    error.Desc = "Zaloguj się od nowa";
                    return StatusCode(405, error);
                }
            }
            catch
            {
                error.Err = "Nieprawidlowy token";
                error.Desc = "Wprowadz token jeszcze raz";
                return StatusCode(405, error);
            }
            List<string> list1 = new List<string>();
            var id = _tokenHelper.GetIdByToken(token);
            var teacher = await _userHelper.ReturnUserByID(id);
            if(teacher.userRole != 1)
            {
                error.Err = "Nie możesz dodać ucznia do klasy";
                error.Desc = "Nie jesteś nauczycielem";
                return StatusCode(405, error);
            }
            try
            {
                _class = database.GetCollection<Class>(addToClassDTO.ClassName);
            }
            catch
            {
                error.Err = "Niepoprawna nazwa klasy";
                error.Desc = "Wprowadź poprawną nazwę klasy";
                return StatusCode(409, error);
            }
            var document = await _class.Find<Class>(x => x.className == addToClassDTO.ClassName).FirstOrDefaultAsync();
            if(document == null)
            {
                error.Err = "Niepoprawna nazwa klasy";
                error.Desc = "Wprowadź poprawną nazwę klasy";
                return StatusCode(409, error);
            }
            var filter = Builders<Class>.Filter.Eq(c => c.Id, document.Id);
            try
            {
                if(_userHelper.DoesUserExist(addToClassDTO.UserToAddID))
                {
                    for (int i = 0; i < document.members.Count; i++)
                    {
                        if(document.members.Contains(addToClassDTO.UserToAddID))
                        {
                            error.Err = "Ten użytkownik już należy do klasy";
                            error.Desc = "Nie możesz dodać użytkownika poraz drugi";
                            return StatusCode(409, error);
                        }
                    }
                    document.members.Add(addToClassDTO.UserToAddID);
                    document.membersAmount++;
                    await _class.ReplaceOneAsync(filter, document);
                    return Ok(document);
                }
                else
                {
                    error.Err = "Niepoprawne ID uzytkownika";
                    error.Desc = "Wprowadz poprawne ID uzytkownika";
                    return StatusCode(409, error);
                }
            }
            catch
            {
                error.Err = "Niepoprawne ID uzytkownika";
                error.Desc = "Wprowadz poprawne ID uzytkownika";
                return StatusCode(409, error);
            }
        }

    }
}