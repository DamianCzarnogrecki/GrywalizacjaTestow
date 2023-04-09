using BlazorApp1.Shared;

namespace BlazorApp1.Client.Services
{
    public interface IPlayerService
    {
        List<player> Players { get; set; }
        int[] NrOfLands { get; set; }
        int[] CorrectAnswersCount { get; set; }
        int[] IncorrectAnswersCount { get; set; }
        Task<float> GetCorrectAnswersCount();
        Task<double> GetAvgNrOfCities();
        Task<int> GetHardestQuestionNr();
        Task<double> GetAvgAnswerTime();
        Task Register(EnteredUserData user);
        Task GetPlayers();
        Task GetLandsPerPlayer();
        Task GetPlayerAnswersCount(bool correctness);
        bool? RegisterError { get; set; }
    }
}