using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

public class AnswerController : MonoBehaviour
{
    public int questionID, answerID;
    [SerializeField]
    public Image answerIcon;
    [SerializeField]
    public Sprite goodAnswerIcon, wrongAnswerIcon;
    public Timer timer;
    private int playerID;
    public PlayerDataController playerDataController;
    private TextMeshProUGUI nextTownPossibleText;

    public void StartCheckingAnswer()
    {
        StartCoroutine(CheckAnswer());
        GameObject answerButton = GameObject.FindGameObjectWithTag("NextQuestion");

        answerButton.GetComponent<Image>().enabled = true;
        answerButton.GetComponent<Button>().enabled = true;
        answerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = true;

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("AnswerButton");
        foreach (GameObject button in buttons)
        {
            Button buttonComponent = button.GetComponent<Button>();
            buttonComponent.enabled = false;

            Image imageComponent = button.GetComponent<Image>();
            imageComponent.color = new Color32(96, 96, 96, 196);
        }
    }

    IEnumerator CheckAnswer()
    {
        string url = "https://localhost:7060/api/question/question/" + questionID.ToString() + "/answer/" + answerID.ToString();

        using (HttpClient client = new HttpClient())
        {
            //potrzebne naglowki
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = client.GetAsync(url);
            while (!request.IsCompleted) yield return null;

            HttpResponseMessage response = request.Result;
            response.EnsureSuccessStatusCode();

            var readTask = response.Content.ReadAsStringAsync();
            while (!readTask.IsCompleted) yield return null;

            string content = readTask.Result;
            int result = int.Parse(content);

            if(result == 1) answerIcon.color = new Color32(0, 255, 0, 196);
            else answerIcon.color = new Color32(255, 0, 0, 196);

            yield return PostAnswer();
        }
    }

    IEnumerator PostAnswer()
    {
        string url = "https://localhost:7060/api/question/answer";
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        playerID = GameObject.Find("PlayerData").GetComponent<PlayerDataController>().playerid;

        using (HttpClient client = new HttpClient())
        {
            var json = $"{{\"data_player_id\": {playerID}, \"data_question_id\": {questionID}, \"data_seconds_spent\": {timer.passedSeconds}, \"data_answer_id\": {answerID}}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            var task = client.PostAsync(url, content);
            yield return new WaitUntil(() => task.IsCompleted);
            response = task.Result;
        }

        playerDataController = GameObject.Find("PlayerData").GetComponent<PlayerDataController>();
        playerDataController.answerCount += 1;

        if(playerDataController.landsCount * 40 <= playerDataController.answerCount)
        {
            nextTownPossibleText = GameObject.Find("NextTownPossibleText").GetComponent<TextMeshProUGUI>();
            nextTownPossibleText.enabled = true;
        } 
    }
}
