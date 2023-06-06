using BlazorApp1.Client.Services;
using BlazorApp1.Shared;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/question")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private Context context;

        public QuestionController(
            Context context
        )
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<QuestionAndAnswers>> GetQuestionWithAnswers()
        {
            int questionCount = await context.question.CountAsync();
            int randomIndex = new Random().Next(questionCount);
            question randomQuestion = await context.question.Skip(randomIndex).FirstOrDefaultAsync();

            List<answer> questionAnswers = await context.question_answer.Where(qa => qa.question_id == randomQuestion.id).Include(qa => qa.answer).Select(qa => qa.answer).ToListAsync();

            if (randomQuestion == null || questionAnswers == null) return NotFound();

            return new QuestionAndAnswers { Question = randomQuestion, Answers = questionAnswers };
        }

        [HttpGet("question/{question_id}/answer/{answer_id}")]
        public async Task<ActionResult<int>> CheckIfAnswerIsCorrect(int question_id, int answer_id)
        {
            var questionAnswer = await context.question_answer.SingleOrDefaultAsync(qa => qa.question_id == question_id && qa.answer_id == answer_id);
            //if (questionAnswer == null) return NotFound();
            return questionAnswer.is_correct;
        }

        [HttpPost("answer")]
        public async Task<IActionResult> AnswerQuestion([FromBody] AnswerQuestionData data)
        {
            var answeredQuestion = new answered_question
            {
                player_id = data.data_player_id,
                question_id = data.data_question_id,
                answered_on = DateTime.Now,
                seconds_spent = data.data_seconds_spent
            };
            context.answered_question.Add(answeredQuestion);
            await context.SaveChangesAsync();

            var givenAnswer = new given_answer
            {
                answered_question_id = answeredQuestion.id,
                question_answer_id = data.data_answer_id
            };
            context.given_answer.Add(givenAnswer);
            await context.SaveChangesAsync();

            return Ok();
        }

        public class AnswerQuestionData
        {
            public int data_player_id { get; set; }
            public int data_question_id { get; set; }
            public int data_seconds_spent { get; set; }
            public int data_answer_id { get; set; }
        }
    }
}
