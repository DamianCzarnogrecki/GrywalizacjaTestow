using BlazorApp1.Client.Services;
using BlazorApp1.Shared;
using Microsoft.AspNetCore.Mvc;

using System.Data.Entity;

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

            if (questionAnswer == null) return NotFound();

            return questionAnswer.is_correct;
        }


    }
}
