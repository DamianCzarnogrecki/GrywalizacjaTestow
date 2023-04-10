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
    private List<string> answerTexts = new List<string>(); 
    private List<int> answerIDs = new List<int>();
    public GameObject answerPrefab;
    public GameObject answersContainer;
    private int questionID;

    void Start()
    {
        StartCoroutine(GetQuestion());
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
                answerObject.GetComponent<AnswerController>().id = answerIDs[i];
            }
        }
    }

    public async Task<int> CheckAnswer(int answerID)
    {
        string url = "https://localhost:7060/api/question/question/" + questionID.ToString() + "/answer/" + answerID.ToString();

        using (HttpClient client = new HttpClient())
        {
            //potrzebne naglowki
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            int result = int.Parse(content);

            //TEST
            Debug.Log(result);
            return result;
        }
    }
}
