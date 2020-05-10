using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileStorageAPI.DTOs;
using HomeSchoolAPI.APIRespond;
using HomeSchoolAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FileStorageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeworkFilesController : ControllerBase
    {
        private BlobServiceClient blobServiceClient;
        private IConfiguration _configuration;
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private Error error;
        private String token;
        private string connectionString;
        public HomeworkFilesController(IConfiguration configuration, IApiHelper apiHelper, ITokenHelper tokenHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }

        [HttpPost("returnBySenderID")]
        public async Task<FileStreamResult> ReturnFilesFromHomework(ReturnBySenderIDDTO returnBySender)
        {
            var file = await _apiHelper.ReturnFileBySenderID(returnBySender.classID, returnBySender.fileID);
            return file;
        }

        [HttpPost("{classID}")]
        public async Task<IActionResult> UploadFileToHomework(string classID, IFormFile file)
        {
            // #region TokenValidation
            // try
            // {
            //     token = HttpContext.Request.Headers["Authorization"];
            //     token = token.Replace("Bearer ", "");
            //     if (!_tokenHelper.IsValidateToken(token))
            //     {
            //         error.Err = "Token wygasł";
            //         error.Desc = "Zaloguj się od nowa";
            //         return StatusCode(405, error);
            //     }
            // }
            // catch
            // {
            //     error.Err = "Nieprawidlowy token";
            //     error.Desc = "Wprowadz token jeszcze raz";
            //     return StatusCode(405, error);
            // }   
            // var id = _tokenHelper.GetIdByToken(token);
            // #endregion
            var id = "5eb548822838553f9038b45f";
            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj.members.Contains(id))
            {
                var subject = await _apiHelper.ReturnSubjectByTeacherID(classID, id);
                if(subject == null)
                {
                    error.Err = "Nieprawidlowe id klasy lub nie jestes nauczycielem przedmiotu";
                    error.Desc = "Nie mozesz dodac pliku do zadania";
                    return StatusCode(405, error);
                }
                var homeworkToReturn = await _apiHelper.UploadFileToHomework(file, classID, id);
                return Ok(homeworkToReturn);
            }
            else
            {
                error.Err = "Nie należysz do klasy";
                error.Desc = "Nie mozesz dodac pliku do zadania";
                return StatusCode(405, error);
            }
        }

    }
}