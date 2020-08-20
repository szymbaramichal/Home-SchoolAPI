using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomeSchoolCore.APIRequest;
using HomeSchoolCore.APIRespond;
using HomeSchoolCore.Filters;
using HomeSchoolCore.Helpers;
using HomeSchoolCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeSchoolAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizesController : ControllerBase
    {
        private IApiHelper _apiHelper;
        private ITokenHelper _tokenHelper;
        private Error error;
        public QuizesController(IApiHelper apiHelper, ITokenHelper tokenHelper)
        {
            error = new Error();
            _apiHelper = apiHelper;
            _tokenHelper = tokenHelper;
        }

        /// <summary>
        /// Creating quiz for subject by subjectTeacher.
        /// </summary>
        [HttpPost("create")]
        [TokenAuthorization]
        public async Task<IActionResult> CreateQuiz(CreateQuizDTO createQuiz)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);

            var subject = await _apiHelper.ReturnSubjectBySubjectID(createQuiz.classID, createQuiz.subjectID);

            if(subject == null)
            {
                error.Err = "Nie znaleziono przedmiotu w klasie o podanym ID.";
                error.Desc = "Wprowadź poprawne wartości ID dla klasy i przedmiotu.";
                return NotFound();
            }

            if(subject.teacherID != id)
            {
                error.Err = "Nie jesteś nauczycielem tego przedmiotu.";
                error.Desc = "Nie możesz dodać quizu.";
                return NotFound();
            }
        
            Quiz quiz = new Quiz();
            quiz.classID = createQuiz.classID;
            quiz.subjectID = createQuiz.subjectID;
            quiz.CreateDate = DateTime.Now;
            quiz.StartDate = createQuiz.StartDate;
            quiz.FinishDate = createQuiz.FinishDate;
            quiz.questions = createQuiz.questions;
            quiz.amountOfQuestions = createQuiz.questions.Count;
            quiz.status = "ACTIVE";

            var isQuizAdded = await _apiHelper.IsQuizAdded(quiz);

            return Ok(quiz);
        }

        /// <summary>
        /// Return all quizes for class.
        /// </summary>
        [HttpGet("getAllQuizes/{classId}")]
        [TokenAuthorization]
        public async Task<ActionResult<List<Quiz>>> GetAllActiveQuizesForClass(string classId)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);

            var isUserInClass = await _apiHelper.DoesUserBelongToClass(id, classId);
        
            if(!isUserInClass)
            {
                error.Err = "Nie należysz do podanej klasy";
                error.Desc = "Nie możesz pobrać quizów";
                return BadRequest(error);
            }

            List<Quiz> quizes = new List<Quiz>();
            quizes = await _apiHelper.ReturnAllActiveQuizesForClass(classId);

            return quizes;
        }

        [HttpPost("completeQuiz")]
        [TokenAuthorization]
        public async Task<IActionResult> CompleteQuiz(CompleteQuizDTO completeQuiz)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);

            int correctAnswersAmount = 0;

            var quiz = await _apiHelper.ReturnQuizById(completeQuiz.classId, completeQuiz.quizId);

            if(quiz == null)
            {
                error.Err = "Nie znaleziono quizu o podanym ID";
                error.Desc = "Wprowadź poprawne ID klasy i quizu";
                return NotFound(error);
            }

            var isUserInClass = await _apiHelper.DoesUserBelongToClass(id, completeQuiz.classId);

            if(!isUserInClass)
            {
                error.Err = "Nie należysz do tej klasy";
                error.Desc = "Wprowadź poprawne ID klasy i quizu";
                return NotFound(error);
            }

            for (int i = 0; i < quiz.questions.Count; i++)
            {
                var question = quiz.questions[i];
                if(completeQuiz.answers[i] == question.correctAnswer)
                {
                    correctAnswersAmount++;
                }
            }

            ResponseToQuiz responseToQuiz = new ResponseToQuiz();
            responseToQuiz.correctAnswers = correctAnswersAmount;
            responseToQuiz.executonerId = id;
            responseToQuiz.classId = completeQuiz.classId;
            responseToQuiz.FinishDate = DateTime.Now;
            responseToQuiz.percentageOfCorrectAnswers = (double)correctAnswersAmount/quiz.questions.Count;
            responseToQuiz.quizId = quiz.Id;

            var isAnswersSaved = await _apiHelper.SaveAnswersToQuiz(responseToQuiz);

            CompleteQuiz completeQuizToReturn = new CompleteQuiz();
            completeQuizToReturn.percentageOfCorrectAnswers = responseToQuiz.percentageOfCorrectAnswers;

            return Ok(completeQuizToReturn);
        }
    }
}