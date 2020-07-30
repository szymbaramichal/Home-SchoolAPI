using System;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Filters;
using HomeSchoolCore.Helpers;
using HomeSchoolCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace TextChatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TextChatController : ControllerBase
    {
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private String token;
        private Error error;
        public TextChatController(ITokenHelper tokenHelper, IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
            error = new Error();
        }

        [HttpPost("sendMessage")]
        [TokenAuthorization]
        public async Task<IActionResult> SendMessage(SendMessageDTO sendMessage)
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
            #endregion
            var id = _tokenHelper.GetIdByToken(token);
            var user = await _apiHelper.ReturnUserByID(id);
            var classObj = await _apiHelper.ReturnClassByID(sendMessage.classID);
            if(classObj == null || !classObj.members.Contains(id))
            {
                error.Err = "Nie możesz wysłać wiadomości";
                error.Desc = "Nie należysz do tej klasy";
                return StatusCode(405, error);
            }
            TextMessage textMessage = new TextMessage
            {
                messageID = 0,
                msessage = sendMessage.message,
                senderName = user.name,
                senderSurname = user.surrname,
                sendTime = DateTime.Now
            };
            var message = await _apiHelper.SendMessage(sendMessage.subjectID, textMessage);
            return Ok(textMessage);
        }
        
        [HttpGet("getLastMessages/{classID}/{subjectID}")]
        public async Task<IActionResult> GetMessages(string subjectID, string classID)
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
            #endregion

            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj == null || !classObj.members.Contains(id))
            {
                error.Err = "Nie możesz przeczytać wiadomości";
                error.Desc = "Nie należysz do tej klasy";
                return StatusCode(405, error);
            }
            var messages = await _apiHelper.ReturnLastMessages(subjectID);
            TextMessagesToReturn textMessages = new TextMessagesToReturn
            {
                messages = messages
            };
            return Ok(textMessages);
        }

        [HttpGet("getNewerMessages/{messageID}/{classID}/{subjectID}")]
        public async Task<IActionResult> GetNewerMessages(string messageID, string classID, string subjectID)
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
            #endregion

            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj == null || !classObj.members.Contains(id))
            {
                error.Err = "Nie możesz przeczytać wiadomości";
                error.Desc = "Nie należysz do tej klasy";
                return StatusCode(405, error);
            }
            int messageNumber;
            try
            {
                messageNumber = Convert.ToInt32(messageID);
            }
            catch
            {
                error.Err = "ID wiadomości musi być liczbą";
                error.Desc = "Nie możesz dodać takiej liczby";
                return StatusCode(405, error);
            }
            var messages = await _apiHelper.ReturnNewerMessages(messageNumber, subjectID);
            TextMessagesToReturn textMessages = new TextMessagesToReturn
            {
                messages = messages
            };
            return Ok(textMessages);
        }
        
        [HttpGet("getOlderMessages/{messageID}/{classID}/{subjectID}")]
        public async Task<IActionResult> GetOlderMessages(string messageID, string subjectID, string classID)
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
            #endregion

            var id = _tokenHelper.GetIdByToken(token);
            var classObj = await _apiHelper.ReturnClassByID(classID);
            if(classObj == null || !classObj.members.Contains(id))
            {
                error.Err = "Nie możesz przeczytać wiadomości";
                error.Desc = "Nie należysz do tej klasy";
                return StatusCode(405, error);
            }
            int messageNumber;
            try
            {
                messageNumber = Convert.ToInt32(messageID);
            }
            catch
            {
                error.Err = "ID wiadomości musi być liczbą";
                error.Desc = "Nie możesz dodać takiej liczby";
                return StatusCode(405, error);
            }
            var messages = await _apiHelper.ReturnOlderMessages(messageNumber, subjectID);

            TextMessagesToReturn textMessages = new TextMessagesToReturn
            {
                messages = messages
            };
            return Ok(textMessages);
        }
    }
}