using System.Net.Http.Json;
using BlazorApp1.Client.Services;
using BlazorApp1.Shared;

namespace BlazorApp1.Client
{
    public class PlayerService : IPlayerService
    {
        static HttpClient http = new HttpClient();
        public List<player> Players { get; set; } = new List<player>();
        public int[] NrOfLands { get; set; }
        public int[] CorrectAnswersCount { get; set; }
        public int[] IncorrectAnswersCount { get; set; }
        public bool? RegisterError { get; set; } = null;

        public async Task Register(EnteredUserData user)
        {
            var response = await http.PostAsJsonAsync("https://localhost:7060/api/register", user);
            RegisterError = response.IsSuccessStatusCode;
        }

        public async Task GetPlayers()
        {
            var response = await http.GetFromJsonAsync<List<player>>("https://localhost:7060/api/getplayers");
            Players = response;
        }

        public async Task GetLandsPerPlayer()
        {
            var response = await http.GetFromJsonAsync<int[]>("https://localhost:7060/api/getlandsperplayer");
            NrOfLands = response;
        }

        public async Task GetPlayerAnswersCount(bool answerCorrectness)
        {
            int[] response = await http.GetFromJsonAsync<int[]>($"https://localhost:7060/api/getplayeranswerscount/{answerCorrectness}");
            if (answerCorrectness) CorrectAnswersCount = response;
            else IncorrectAnswersCount = response;
        }

        public async Task<float> GetCorrectAnswersCount()
        {
            float response = await http.GetFromJsonAsync<float>("https://localhost:7060/api/getcorrectanswerscount");
            return response;
        }

        public async Task<double> GetAvgNrOfCities()
        {
            double response = await http.GetFromJsonAsync<double>("https://localhost:7060/api/getavgnrofcities");
            return response;
        }
        public async Task<int> GetHardestQuestionNr()
        {
            int response = await http.GetFromJsonAsync<int>("https://localhost:7060/api/gethardestquestionnr");
            return response;
        }

        public async Task<double> GetAvgAnswerTime()
        {
            double response = await http.GetFromJsonAsync<double>("https://localhost:7060/api/getavganswertime");
            return response;
        }

        public int[] GetNrOfLandsArray()
        {
            return NrOfLands;
        }
        public int[] GetCorrectAnswersCountArray()
        {
            return CorrectAnswersCount;
        }

        public int[] GetIncorrectAnswersCountArray()
        {
            return IncorrectAnswersCount;
        }
    }
}