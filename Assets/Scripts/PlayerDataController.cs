using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Globalization;
using System.Linq;

public class PlayerDataController : MonoBehaviour
{
    public static PlayerDataController PlayerDataControllerInstance { get; private set; }
    public int playerid = 0;
    public int landsCount = 0;
    public int answerCount = 0;
    public float correctAnswerPercent = 0;
    public LoginController loginController;
    private TextMeshProUGUI currentNrOfTownsText;
    private TextMeshProUGUI currentEpochText;
    private string currentEpoch;

    private void Awake() 
    { 
        //singleton: usuniecie ewentualnych innych instancji
        if (PlayerDataControllerInstance != null && PlayerDataControllerInstance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            PlayerDataControllerInstance = this; 
        } 
    }

    public void GetID()
    {
        //loginController = GameObject.Find("LoginController").GetComponent<LoginController>();
        playerid = loginController.playerid;
    }

    public IEnumerator GetLandsCount()
    {
        UnityWebRequest request = UnityWebRequest.Get($"https://localhost:7060/api/getplayerlandscount/{playerid}");
        yield return request.SendWebRequest();
        int.TryParse(request.downloadHandler.text, out landsCount);
        currentNrOfTownsText = GameObject.Find("PlayerInfoPanel").transform.Find("CurrentNrOfTowns").GetComponent<TextMeshProUGUI>();
        currentNrOfTownsText.text = landsCount.ToString();
    }

    public IEnumerator GetEpoch()
    {
        //pobranie procenta poprawnych odpowiedzi
        UnityWebRequest request = UnityWebRequest.Get($"https://localhost:7060/api/getcorrectanswerscount/{playerid}");
        yield return request.SendWebRequest();
        float.TryParse(request.downloadHandler.text, NumberStyles.Float, CultureInfo.InvariantCulture, out correctAnswerPercent);
        correctAnswerPercent *= 100;

        //wyliczenie epoki
        Epoch.SingleEpoch suitableEpoch = Epoch.Epochs.Where(epoch => epoch.townsRequired <= landsCount && epoch.correctAnswerRatioRequired <= correctAnswerPercent).OrderByDescending(epoch => epoch.correctAnswerRatioRequired).FirstOrDefault();
        currentEpoch = suitableEpoch.name;
        currentEpochText = GameObject.Find("PlayerInfoPanel").transform.Find("CurrentEpoch").GetComponent<TextMeshProUGUI>();
        currentEpochText.text = currentEpoch;
    }

    public IEnumerator GetAnswerCount()
    {
        UnityWebRequest request = UnityWebRequest.Get($"https://localhost:7060/api/getallanswerscountofaplayer/{playerid}");
        yield return request.SendWebRequest();
        int.TryParse(request.downloadHandler.text, out answerCount);
    }
}