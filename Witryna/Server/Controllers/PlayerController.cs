using BlazorApp1.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using BlazorApp1.Client.Services;
using System.Data.SqlClient;

namespace BlazorApp1.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public IPlayerService playerService;
        private Context context;

        public PlayerController(
            IConfiguration configuration,
            IPlayerService playerService,
            Context context
        )
        {
            this.configuration = configuration;
            this.playerService = playerService;
            this.context = context;
        }

        [HttpGet("getplayers")]
        public async Task<ActionResult<List<player>>> GetPlayers()
        {
            var zwrocone = context.Player.ToList();
            return Ok(zwrocone);
        }

        [HttpPost("register")]
        public async Task<bool> Register(EnteredUserData userData)
        {
            try
            {
                var maxID = context.Player?.OrderByDescending(u => u.id).FirstOrDefault();
                player player = new player();
                if (maxID.id == null) player.id = 1;
                else player.id = maxID.id + 1;
                player.login = userData.Login;
                player.registered_on = DateTime.Now;
                Hashing(userData.Password, out byte[] hash, out byte[] salt);
                player.password_hash = hash;
                player.password_salt = salt;
                context.Player?.Add(player);
                await context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            catch(SqlException e)
            {
                //UWAGA
                //dokonczyc przechwytywanie
                return await Task.FromResult(false);
            }
            catch(Exception e)
            {
                //UWAGA
                //dokonczyc przechwytywanie
                return await Task.FromResult(false);
            }
        }

        private void Hashing(string haslo, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(haslo));
            }
        }

        [HttpGet("getlandsperplayer")]
        public async Task<ActionResult<int[]>> GetLandsPerPlayer()
        {
            var zwrocone = from player in context.Player
                           join land in context.land on player.id equals land.player_id into landsGroup
                           from land in landsGroup.DefaultIfEmpty()
                           group land by player.id into g
                           select g.Count(l => l != null);

            return Ok(zwrocone);
        }

        [HttpGet("getcorrectanswerscount")]
        public async Task<ActionResult<List<given_answer>>> GetCorrectAnswersCount()
        {
            var correctAnswers = (from q in context.question_answer
                                  where q.is_correct == true
                                  join g in context.given_answer
                                      on q.id equals g.question_answer_id
                                  select new
                                  {
                                      id = g.id
                                  }).Count();

            var allAnswers = (from a in context.given_answer select a).Count();
            float correctAnswerRatio = (float)correctAnswers / (float)allAnswers;
            return Ok(correctAnswerRatio);
        }

        [HttpGet("getavgnrofcities")]
        public async Task<ActionResult<double>> GetAvgNrOfCities()
        {
            var result = context.Player
                .Join(context.land, p => p.id, l => l.player_id, (p, l) => new { Player = p })
                .GroupBy(x => x.Player.id)
                .Select(g => g.Count())
                .Average();

            return Ok(result);
        }

        [HttpGet("gethardestquestionnr")]
        public async Task<ActionResult<int>> GetHardestQuestionNr()
        {
            var query = from q in context.question
                        join qa in context.question_answer on q.id equals qa.question_id
                        join ga in context.given_answer on qa.id equals ga.question_answer_id
                        group qa by q.id into qag
                        orderby qag.Sum(qa => qa.is_correct ? 1 : 0) ascending
                        select qag.Key;
            return Ok(query.FirstOrDefault());
        }

        [HttpGet("getavganswertime")]
        public async Task<ActionResult<double>> GetAvgAnswerTime()
        {
            double answerTime = (from a in context.answered_question select a.seconds_spent).Average();
            return answerTime;
        }

        [HttpGet("getplayeranswerscount/{answerCorrectness}")]
        public async Task<ActionResult<List<int>>> GetPlayerAnswersCount(bool answerCorrectness)
        {
            var response = from player in context.Player
                           join answeredQuestion in context.answered_question
                               on player.id equals answeredQuestion.player_id into answeredQuestionsGroup
                           from answeredQuestion in answeredQuestionsGroup.DefaultIfEmpty()
                           join givenAnswer in context.given_answer
                               on answeredQuestion.id equals givenAnswer.answered_question_id into givenAnswersGroup
                           from givenAnswer in givenAnswersGroup.DefaultIfEmpty()
                           join questionAnswer in context.question_answer
                               on givenAnswer.question_answer_id equals questionAnswer.id into questionAnswersGroup
                           from questionAnswer in questionAnswersGroup.DefaultIfEmpty()
                           group new { questionAnswer.is_correct } by player.id into playerGroup
                           select playerGroup.Count(qa => qa.is_correct == answerCorrectness);
            return Ok(response.ToArray());
        }
    }
}