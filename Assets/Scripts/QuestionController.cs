using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class QuestionController : MonoBehaviour
{
    private string url = "https://localhost:7060/api/question";
    private string questionText;
    private List<string> answerTexts; 
    private List<int> answerIDs;
    private int questionID;
    [SerializeField]
    private GameObject answerPrefab;
    [SerializeField]
    private GameObject answersContainer;
    public Timer timer;
    public TextMeshProUGUI MustBeLoggedText;
    private PlayerDataController playerDataController;
    public ButtonHider buttonHider;

    void Start()
    {
        //StartGettingQuestion();
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        playerDataController = GameObject.Find("PlayerData").GetComponent<PlayerDataController>();
        MustBeLoggedText = GameObject.Find("MustBeLoggedText").GetComponent<TextMeshProUGUI>();
        buttonHider = GameObject.Find("NextQuestionButton").GetComponent<ButtonHider>();
    }

    public void StartGettingQuestion()
    {
        if(playerDataController.playerid > 0)
        {
            foreach (Transform child in answersContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            answerTexts = new List<string>();
            answerIDs = new List<int>();
            StartCoroutine(GetQuestion());
            timer.passedSeconds = 0;

            MustBeLoggedText.enabled = false;
            buttonHider.HideTheButton();
        }
        else
        {
            MustBeLoggedText.enabled = true;
        }

    }

    IEnumerator GetQuestion()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            questionText = "BŁĄD ZWRÓCENIA PYTANIA";
        }
        else
        {
            var json = JSON.Parse(request.downloadHandler.text);
            questionText = json["question"]["text"];
            questionID = json["question"]["id"];
            

            JSONArray answersArray = json["answers"].AsArray;
            for (int i = 0; i < answersArray.Count; i++)
            {
                JSONObject answer = answersArray[i].AsObject;
                answerTexts.Add(answer["text"].Value);
                answerIDs.Add(answer["id"]);
            }

            TextMeshProUGUI questionTextMesh = GetComponent<TextMeshProUGUI>();
            questionTextMesh.text = questionText;

            //instancjacja i pozycjonowanie przycisku
            float buttonHeight = answersContainer.GetComponent<RectTransform>().rect.height / answerTexts.Count;
            for (int i = 0; i < answerTexts.Count; i++)
            {
                GameObject answerObject = Instantiate(answerPrefab, answersContainer.transform);
                answerObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (-i * buttonHeight)+130);
                answerObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = answerTexts[i];
                answerObject.GetComponent<AnswerController>().answerID = answerIDs[i];
                answerObject.GetComponent<AnswerController>().questionID = questionID;
            }
        }
    }
}
