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
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private Error error;
        private String token;
        public HomeworkFilesController(IApiHelper apiHelper, ITokenHelper tokenHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }

        [HttpPost("returnFileFromHomework")]
        public async Task<FileStreamResult> ReturnFilesFromHomework(ReturnForHomeworkDTO returnForHomework)
        {
            var file = await _apiHelper.ReturnHomeworkFileBySenderID(returnForHomework.classID, returnForHomework.fileID);
            return file;
        }

        [HttpPost("returnFileFromResponse")]
        public async Task<FileStreamResult> ReturnFilesFromResponse(ReturnForResponse returnForResponse)
        {
            var file = await _apiHelper.ReturnHomeworkFileBySenderID(returnForResponse.homeworkID, returnForResponse.fileID);
            return file;
        }

        [HttpPost("uploadToHomework/{classID}")]
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
            var id = "5eb91b284f987a2520d4e3be";
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
                var fileID = await _apiHelper.UploadFileToHomework(file, classID, id);
                return Ok(fileID);
            }
            else
            {
                error.Err = "Nie należysz do klasy";
                error.Desc = "Nie mozesz dodac pliku do zadania";
                return StatusCode(405, error);
            }
        }

        [HttpPost("uploadToResponse/{classID}/{homeworkID}")]
        public async Task<IActionResult> UploadFileToResponse(string homeworkID, string classID,IFormFile file)
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
            var id = "5eb91d3bbbc6d4163081751e";
            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj.members.Contains(id))
            {
                var fileID = await _apiHelper.UploadFileToResponse(file, homeworkID, id);
                if(fileID == null)
                {
                    error.Err = "Złe id zadania";
                    error.Desc = "Nie mozesz dodac pliku do zadania";
                    return StatusCode(405, error);
                }
                return Ok(fileID);
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