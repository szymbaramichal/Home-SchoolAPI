using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileStorageAPI.ApiResponse;
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
        public async Task<FileStreamResult> ReturnFileFromHomework(ReturnForHomeworkDTO returnForHomework)
        {
            #region TokenValidation
            try
            {
                token = HttpContext.Request.Headers["Authorization"];
                token = token.Replace("Bearer ", "");
                if (!_tokenHelper.IsValidateToken(token))
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes("");
                    MemoryStream stream = new MemoryStream(byteArray);
                    return new FileStreamResult(stream, "application/octet-stream");
                }
            }
            catch
            {
                byte[] byteArray = Encoding.ASCII.GetBytes("");
                MemoryStream stream = new MemoryStream(byteArray);
                return new FileStreamResult(stream, "application/octet-stream");
            }   
            #endregion
            var id = _tokenHelper.GetIdByToken(token);
            var subject = await _apiHelper.ReturnSubjectBySubjectID(returnForHomework.classID, returnForHomework.subjectID);
            if(!subject.homeworks.Contains(returnForHomework.homeworkID) || subject.teacherId != id)
            {
                byte[] byteArray = Encoding.ASCII.GetBytes("");
                MemoryStream stream = new MemoryStream(byteArray);
                return new FileStreamResult(stream, "application/octet-stream");
            }
            var file = await _apiHelper.ReturnHomeworkFileBySenderID(returnForHomework.classID, returnForHomework.fileID);
            Response.Headers.Add("fileName", file.fileName);
            Response.Headers.Remove("Access-Control-Expose-Headers");
            Response.Headers.Add("Access-Control-Expose-Headers", "*");
            return new FileStreamResult(file.stream, file.contentType);
        }

        [HttpPost("returnFileFromResponse")]
        public async Task<FileStreamResult> ReturnFileFromResponse(ReturnForResponse returnForResponse)
        {
            #region TokenValidation
            try
            {
                token = HttpContext.Request.Headers["Authorization"];
                token = token.Replace("Bearer ", "");
                if (!_tokenHelper.IsValidateToken(token))
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes("");
                    MemoryStream stream = new MemoryStream(byteArray);
                    return new FileStreamResult(stream, "application/octet-stream");
                }
            }
            catch
            {
                byte[] byteArray = Encoding.ASCII.GetBytes("");
                MemoryStream stream = new MemoryStream(byteArray);
                return new FileStreamResult(stream, "application/octet-stream");
            }   
            #endregion
            var id = _tokenHelper.GetIdByToken(token);
            var file = await _apiHelper.ReturnResponseFileBySenderID(returnForResponse.homeworkID, returnForResponse.fileID);
            if(file.senderID != id)
            {
                byte[] byteArray = Encoding.ASCII.GetBytes("");
                MemoryStream stream = new MemoryStream(byteArray);
                return new FileStreamResult(stream, "application/octet-stream");
            }
            Response.Headers.Add("fileName", file.fileName);
            Response.Headers.Remove("Access-Control-Expose-Headers");
            Response.Headers.Add("Access-Control-Expose-Headers", "*");
            return new FileStreamResult(file.stream, file.contentType);
        }

        [HttpPost("uploadToHomework/{classID}/{subjectID}")]
        public async Task<IActionResult> UploadFileToHomework(string classID, string subjectID, IFormFile file)
        {
            #region TokenValidation
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
            var id = _tokenHelper.GetIdByToken(token);
            #endregion
            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj.members.Contains(id))
            {
                var subject = await _apiHelper.ReturnSubjectBySubjectID(classID, subjectID);
                if(subject == null)
                {
                    error.Err = "Nieprawidlowe id klasy lub nie jestes nauczycielem przedmiotu";
                    error.Desc = "Nie mozesz dodac pliku do zadania";
                    return StatusCode(405, error);
                }
                var fileID = await _apiHelper.UploadFileToHomework(file, classID, id, subjectID);
                FileResponse fileResponse = new FileResponse();
                fileResponse.fileID = fileID;
                return Ok(fileResponse);
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
            #region TokenValidation
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
            var id = _tokenHelper.GetIdByToken(token);
            #endregion
            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj.members.Contains(id))
            {
                var fileID = await _apiHelper.UploadFileToResponse(file, homeworkID, id, "", classID);
                if(fileID == null)
                {
                    error.Err = "Złe id zadania";
                    error.Desc = "Nie mozesz dodac pliku do zadania";
                    return StatusCode(405, error);
                }
                FileResponse fileResponse = new FileResponse();
                fileResponse.fileID = fileID;
                return Ok(fileResponse);
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