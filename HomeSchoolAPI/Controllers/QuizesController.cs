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
        [HttpPost("createQuiz")]
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
                return NotFound(error);
            }

            if(subject.teacherID != id)
            {
                error.Err = "Nie jesteś nauczycielem tego przedmiotu.";
                error.Desc = "Nie możesz dodać quizu.";
                return NotFound(error);
            }

            if(DateTime.Compare(createQuiz.FinishDate, DateTime.Now) < 0)
            {
                error.Err = "Data końcowa jest wcześniejsza jak obecna.";
                error.Desc = "Nie możesz dodać quizu.";
                return NotFound(error);
            }

            var isQuizAdded = await _apiHelper.IsQuizAdded(createQuiz);

            return Ok(isQuizAdded);
        }

        /// <summary>
        /// Return all quizes for subject.
        /// </summary>
        [HttpGet("getAllQuizesForSubject/{classID}/{subjectID}")]
        [TokenAuthorization]
        public async Task<ActionResult<QuizesToReturn>> GetQuizesForSubject(string classID, string subjectID)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);

            var isUserInClass = await _apiHelper.DoesUserBelongToClass(id, classID);
        
            if(!isUserInClass)
            {
                error.Err = "Nie należysz do podanej klasy";
                error.Desc = "Nie możesz pobrać quizów";
                return BadRequest(error);
            }

            var quizes = await _apiHelper.ReturnQuizesForSubject(classID, subjectID, id);
            if(quizes == null) return NoContent();

            return quizes;
        }



        /// <summary>
        /// Get quiz questions with answers.
        /// </summary>
        [HttpGet("getQuizQuestions/{classID}/{quizID}")]
        [TokenAuthorization]
        public async Task<ActionResult<List<QuestionToReturn>>> GetQuestionsForQuiz(string classID, string quizID)
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);

            var isUserInClass = await _apiHelper.DoesUserBelongToClass(id, classID);
        
            if(!isUserInClass)
            {
                error.Err = "Nie należysz do podanej klasy";
                error.Desc = "Nie możesz pobrać quizów";
                return BadRequest(error);
            }

            List<QuestionToReturn> questionToReturn = new List<QuestionToReturn>();
            questionToReturn = await _apiHelper.ReturnQuestionsForQuiz(classID, quizID);

            if(questionToReturn == null)
            {
                error.Err = "Niepoprawne id quizu.";
                error.Desc = "Nie możesz pobrać pytań.";
                return NotFound(error);
            }

            return questionToReturn;
        }

        /// <summary>
        /// Get all answers for student.
        /// </summary>
        [HttpGet("getAnswersForStudent")]
        [TokenAuthorization]
        public async Task<ActionResult<List<AnswerToReturn>>> GetAnswersForStudent()
        {
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);

            var answers = await _apiHelper.GetAnswersForStudent(id);

            if(answers == null)
            {
                error.Err = "Nie jesteś nauczycielem";
                error.Desc = "Nie możesz pobrać swoich odpowiedzi";
                return NotFound(error);
            }

            return answers;
        }


        /// <summary>
        /// Send answers for quiz.
        /// </summary>
        [HttpPost("completeQuiz")]
        [TokenAuthorization]
        public async Task<IActionResult> CompleteQuiz(CompleteQuizDTO completeQuiz)
        {
            //Dodac: brak mozliwosci wysylania odpowiedzi przez nauczyciela,
            //sprawdzenie startDate, ID zamiast Id
            string token = HttpContext.Request.Headers["Authorization"];
            token = token.Replace("Bearer ", string.Empty);

            var id = _tokenHelper.GetIdByToken(token);

            int correctAnswersAmount = 0;

            var isUserInClass = await _apiHelper.DoesUserBelongToClass(id, completeQuiz.classId);
            if(!isUserInClass)
            {
                error.Err = "Nie należysz do tej klasy";
                error.Desc = "Wprowadź poprawne ID klasy i quizu";
                return NotFound(error);
            }

            var usr = await _apiHelper.ReturnUserByID(id);
            if(usr.userRole == 1)
            {
                error.Err = "Nie możesz odesłać quizu.";
                error.Desc = "Jako nauczyciel nie możesz odesłać quizu.";
                return NotFound(error);
            }

            var quiz = await _apiHelper.ReturnQuizById(completeQuiz.classId, completeQuiz.quizId);
            if(quiz == null)
            {
                error.Err = "Nie znaleziono quizu o podanym ID";
                error.Desc = "Wprowadź poprawne ID klasy i quizu";
                return NotFound(error);
            }

            var questions = await _apiHelper.ReutrnCorrectQuizQuestions(completeQuiz.classId, completeQuiz.quizId);

            if(DateTime.Compare(quiz.FinishDate, DateTime.Now) < 0)
            {
                error.Err = "Nie mozesz wyslac odpowiedzi do quizu.";
                error.Desc = "Czas na odsylanie odpowiedzi minął.";
                return NotFound(error);
            }

            if(DateTime.Compare(quiz.StartDate, DateTime.Now) > 0)
            {
                error.Err = "Nie mozesz wyslac odpowiedzi do quizu.";
                error.Desc = "Quiz się nie rozpoczął.";
                return NotFound(error);
            }

            if(completeQuiz.answers.Count != quiz.amountOfQuestions)
            {
                error.Err = "Nie mozesz wyslac odpowiedzi do quizu.";
                error.Desc = "Nie odpowiedziałeś na wszystkie pytania.";
                return NotFound(error);
            }

            for (int i = 0; i < questions.Questions.Count; i++)
            {
                var question = questions.Questions[i];
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
            responseToQuiz.percentageOfCorrectAnswers = (double)correctAnswersAmount/questions.Questions.Count * 100;
            responseToQuiz.quizId = quiz.Id;

            bool isAnswerCorrect = await _apiHelper.SaveAnswersToQuiz(responseToQuiz);

            if(!isAnswerCorrect)
            {
                error.Err = "Nie mozesz wyslac odpowiedzi do quizu.";
                error.Desc = "Już raz je przesłałeś.";
                return NotFound(error);
            }

            CompleteQuiz completeQuizToReturn = new CompleteQuiz();
            completeQuizToReturn.PercentageOfCorrectAnswers = responseToQuiz.percentageOfCorrectAnswers;

            return Ok(completeQuizToReturn);
        }
    }
}