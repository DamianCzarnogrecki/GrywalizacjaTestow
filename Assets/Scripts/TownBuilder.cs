using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class TownBuilder : MonoBehaviour
{
    public PlayerDataController playerDataController;
    public Land landData;
    public TextMeshProUGUI nextTownPossibleText;
    public int landID, playerID;

    void Start() {
        landData = gameObject.GetComponent<Land>();
        playerDataController = GameObject.Find("PlayerData").GetComponent<PlayerDataController>();
        nextTownPossibleText = GameObject.Find("NextTownPossibleText").GetComponent<TextMeshProUGUI>();
        landID = landData.ID;
    }
    public void BuildATown()
    {
        playerID = playerDataController.playerid;

        if(playerID <= 0)
        {
            DisplayErrorMessage("Musisz byc zalogowany.");
            return;
        }
        if(!nextTownPossibleText.enabled)
        {
            DisplayErrorMessage("Nie mozesz jeszcze zbudowac kolejnego miasta.");
            return;
        }
        StartCoroutine(GetLandOwnership(landID));
    }

    public IEnumerator GetLandOwnership(int landID)
    {
        string url = "https://localhost:7060/api/checklandownership/" + landID;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            string responseText = webRequest.downloadHandler.text;
            bool isOwned = bool.Parse(responseText);
            BuildTown(isOwned);
        }
    }

    public void BuildTown(bool isOwned)
    {
        if(isOwned) DisplayErrorMessage("Ten teren jest juz zajety.");
        else
        {
            StartCoroutine(ClaimALand());
            var lands = playerDataController.landsCount;
            lands++;
            var currentNrOfTownsText = GameObject.Find("PlayerInfoPanel").transform.Find("CurrentNrOfTowns").GetComponent<TextMeshProUGUI>();
            currentNrOfTownsText.text = "miasta: " + lands.ToString();
            if(lands * 40 > playerDataController.answerCount) nextTownPossibleText.enabled = false;
        }

        var generator = GameObject.Find("LandMapGenerator");
        for (int i = generator.transform.childCount - 1; i >= 0; i--) Destroy(generator.transform.GetChild(i).gameObject);
        generator.GetComponent<LandMapGenerator>().GenerateTheMap();
    }

    public void DisplayErrorMessage(string message)
    {
        GameObject.Find("ErrorMessage").transform.Find("ErrorText").GetComponent<TextMeshProUGUI>().text = message;
        GameObject.Find("ErrorMessage").transform.Find("CloseButton").GetComponent<Button>().enabled = true;
        GameObject.Find("ErrorMessage").transform.Find("CloseButton").GetComponent<Image>().enabled = true;
    }

    IEnumerator ClaimALand()
    {
        playerID = playerDataController.playerid;
        string url = $"https://localhost:7060/api/land/{landID}/player/{playerID}";
        UnityWebRequest request = UnityWebRequest.Put(url, "");
        yield return request.SendWebRequest();
    }
}
