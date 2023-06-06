using BlazorApp1.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using BlazorApp1.Client.Services;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static BlazorApp1.Server.Controllers.QuestionController;

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

        private void Hashing(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
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

        [HttpGet("getplayerlandscount/{id}")]
        public async Task<ActionResult<int>> GetLandsPerPlayer(int id)
        {
            var zwrocone = await context.land.Where(l => l.player_id == id).CountAsync();
            return Ok(zwrocone);
        }

        [HttpGet("getcorrectanswerscount")]
        public async Task<ActionResult<List<given_answer>>> GetCorrectAnswersCount()
        {
            var correctAnswers = (from q in context.question_answer
                                  where q.is_correct == 1
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

        [HttpGet("getcorrectanswerscount/{playerID}")]
        public async Task<ActionResult<float>> GetCorrectAnswersRatio(int playerID)
        {
            var correctAnswers = await (from q in context.question_answer
                                        where q.is_correct == 1
                                        join g in context.given_answer on q.id equals g.question_answer_id
                                        join aq in context.answered_question on g.answered_question_id equals aq.id
                                        where aq.player_id == playerID
                                        select g).CountAsync();

            var allAnswers = await (from a in context.given_answer
                                    join aq in context.answered_question on a.answered_question_id equals aq.id
                                    where aq.player_id == playerID
                                    select a).CountAsync();

            float correctAnswerRatio = allAnswers > 0 ? (float)correctAnswers / (float)allAnswers : 0f;
            return Ok(correctAnswerRatio);
        }

        [HttpGet("getallanswerscountofaplayer/{playerID}")]
        public async Task<ActionResult<int>> GetAllAnswersCountOfAPlayer(int playerID)
        {
            var allAnswers = await (from a in context.given_answer
                                    join aq in context.answered_question on a.answered_question_id equals aq.id
                                    where aq.player_id == playerID
                                    select a).CountAsync();

            return Ok(allAnswers);
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
                        orderby qag.Sum(qa => qa.is_correct == 1 ? 1 : 0) ascending
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
                           select playerGroup.Count(qa => qa.is_correct == Convert.ToByte(answerCorrectness));
            return Ok(response.ToArray());
        }

        [HttpPost("login")]
        public async Task<int> Login([FromBody] PlayerData playerDataFromUser)
        {
            var playerDataFromDatabase = await context.Player.FirstOrDefaultAsync(p => p.login == playerDataFromUser.login);
            if (playerDataFromDatabase == null) return 0;

            if (playerDataFromDatabase.password_hash != null && playerDataFromDatabase.password_salt != null)
            {
                if(
                    PasswordVerification(
                        playerDataFromUser.password,
                        playerDataFromDatabase.password_hash,
                        playerDataFromDatabase.password_salt
                    )
                ) return playerDataFromDatabase.id;
            }
            return 0;
        }

        private bool PasswordVerification(string haslo, byte[] hash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(haslo));
                return computedHash.SequenceEqual(hash);
            }
        }

        public class PlayerData
        {
            public string login { get; set; }
            public string password { get; set; }
        }

        [HttpGet("lands")]
        public async Task<ActionResult<IEnumerable<LandData>>> GetLandIDs()
        {
            var lands = await context.land
            .Select(l => new LandData
            {
                id = l.id,
                player_id = l.player_id,
                login = l.player.login,
            })
            .ToListAsync();
            return Ok(lands);
        }

        public class LandData
        {
            public int id { get; set; }
            public int? player_id { get; set; }
            public string login { get; set; }
        }

        [HttpGet("getidratiopairs")]
        public async Task<ActionResult<IEnumerable<CorrectAnswerRatio>>> GetIdRatioPairs()
        {
            var playerCorrectAnswers = await context.Player
                .Select(p => new CorrectAnswerRatio
                {
                    PlayerID = p.id,
                    Ratio = context.given_answer
                        .Where(ga => ga.answered_question.player_id == p.id && ga.question_answer.is_correct == 1)
                        .Count() / Math.Max((float)context.given_answer
                            .Where(ga => ga.answered_question.player_id == p.id)
                            .Count(), 1)
                })
                .ToListAsync();

            return Ok(playerCorrectAnswers);
        }

        public class CorrectAnswerRatio
        {
            public int PlayerID { get; set; }
            public float Ratio { get; set; }
        }

        [HttpGet("getlandsofplayers")]
        public async Task<ActionResult<List<LandOfPlayer>>> GetLandsOfPlayers()
        {
            var playerLands = from player in context.Player
                              join land in context.land on player.id equals land.player_id into landsGroup
                              from land in landsGroup.DefaultIfEmpty()
                              group land by player.id into g
                              select new LandOfPlayer { PlayerId = g.Key, NumberOfLands = g.Count(l => l != null) };

            return Ok(playerLands.ToList());
        }

        public class LandOfPlayer
        {
            public int PlayerId { get; set; }
            public int NumberOfLands { get; set; }
        }

        [HttpGet("getplayerids")]
        public async Task<ActionResult<List<int>>> GetPlayerIds()
        {
            var playerIds = await context.Player.Select(player => player.id).OrderBy(id => id).ToListAsync();
            return Ok(playerIds);
        }

        
        [HttpGet("checklandownership/{landID}")]
        public async Task<ActionResult<bool>> CheckLandOwnership(int landID)
        {
            var land = await context.land.FindAsync(landID);
            if (land == null) return false;
            return land.player_id != null;
        }

        [HttpPut("land/{landID}/player/{playerID}")]
        public async Task<IActionResult> ClaimALand(int landID, int playerID)
        {
            var land = await context.land.FindAsync(landID);
            land.player_id = playerID;
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}