using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine.UI;
using TMPro;

public class AnswerController : MonoBehaviour
{
    public int questionID, answerID;
    [SerializeField]
    public Image answerIcon;
    [SerializeField]
    public Sprite goodAnswerIcon, wrongAnswerIcon;

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
            if (buttonComponent != null)
            {
                buttonComponent.enabled = false;
            }

            Image imageComponent = button.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.color = new Color32(96, 96, 96, 255);
            }
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
            while (!request.IsCompleted)
            {
                yield return null;
            }

            HttpResponseMessage response = request.Result;
            response.EnsureSuccessStatusCode();

            var readTask = response.Content.ReadAsStringAsync();
            while (!readTask.IsCompleted)
            {
                yield return null;
            }

            string content = readTask.Result;
            int result = int.Parse(content);

            if(result == 1) answerIcon.sprite = goodAnswerIcon;
            else answerIcon.sprite = wrongAnswerIcon;

            yield return null;
        }
    }


}
