using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Networking;
using System.Globalization;

public class LandMapGenerator : MonoBehaviour
{
    public int Rows = 10;
    public int Columns = 10;
    public float TileWidth = 30f;
    public float TileHeight = 30f;
    public GameObject TilePrefab;
    public Vector2 TilesOffset;
    public List<LandOwner> LandOwners;
    public List<MapDataOfAPlayer> AllPlayersMapData;
    private GameObject[,] tiles;
    public List<int> playerIds = new List<int>();
    public List<Sprite> epochTownSprite = new List<Sprite>();

    void Start()
    {
        GenerateTheMap();
    }

    public void GenerateTheMap()
    {
        AllPlayersMapData = new List<MapDataOfAPlayer>();

        tiles = new GameObject[Rows, Columns];

        int tileID = 1;

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                GameObject tile = Instantiate(TilePrefab, transform);

                Land land = tile.GetComponent<Land>();
                land.ID = tileID;

                RectTransform rectTransform = tile.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(TileWidth, TileHeight);
                rectTransform.anchoredPosition = new Vector2(TilesOffset.x + column * TileWidth, TilesOffset.y - row * TileHeight);

                tiles[row, column] = tile;

                tileID++;
            }
        }
        
        AssignLandOwners();
        StartCoroutine(LoadAllPlayersData());
    }

    async void AssignLandOwners()
    {
        await GetLandOwners();
        foreach (GameObject tile in tiles)
        {
            Land land = tile.GetComponent<Land>();
            LandOwner landOwner = LandOwners.FirstOrDefault(lo => lo.id == land.ID);
            if (landOwner != null)
            {
                land.OwnerID = landOwner.player_id ?? 0;
                var ownerTextMesh = tile.transform.Find("OwnerText").gameObject.GetComponent<TextMeshProUGUI>();
                ownerTextMesh.text = landOwner.login ?? "";
                if(land.OwnerID > 0) Destroy(land.GetComponent<Button>());
            }
        }
    }

    async Task<IEnumerator> GetLandOwners()
    {
        string url = "https://localhost:7060/api/lands";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                LandOwners = JsonConvert.DeserializeObject<List<LandOwner>>(jsonString);
            }
        }
        return null;
    }

    public class LandOwner
    {
        public int id { get; set; }
        public int? player_id { get; set; }
        public string login { get; set; }
    }

    public class MapDataOfAPlayer
    {
        public int ID { get; set; }
        public int NrOfLands { get; set; }
        public float CorrectAnswerRatio { get; set; }
        public string Epoch { get; set; }
        public int AnswerCount { get; set; }
    }

    public IEnumerator LoadAllPlayersData()
    {
        yield return StartCoroutine(GetPlayerIds());

        foreach (int playerId in playerIds)
        {
            MapDataOfAPlayer playerData = new MapDataOfAPlayer();
            playerData.ID = playerId;

            UnityWebRequest landsRequest = UnityWebRequest.Get($"https://localhost:7060/api/getplayerlandscount/{playerId}");
            landsRequest.SendWebRequest();
            while (!landsRequest.isDone) yield return null;
            int landsCount;
            if (int.TryParse(landsRequest.downloadHandler.text, out landsCount)) playerData.NrOfLands = landsCount;

            UnityWebRequest correctAnswersRequest = UnityWebRequest.Get($"https://localhost:7060/api/getcorrectanswerscount/{playerId}");
            correctAnswersRequest.SendWebRequest();
            while (!correctAnswersRequest.isDone) yield return null;

            float correctAnswerRatio;

            if (float.TryParse(correctAnswersRequest.downloadHandler.text, NumberStyles.Float, CultureInfo.InvariantCulture, out correctAnswerRatio))
            {
                correctAnswerRatio *= 100;
                playerData.CorrectAnswerRatio = correctAnswerRatio;
            }

            UnityWebRequest answerCountRequest = UnityWebRequest.Get($"https://localhost:7060/api/getallanswerscountofaplayer/{playerId}");
            answerCountRequest.SendWebRequest();
            while (!answerCountRequest.isDone) yield return null;
            int answerCount;
            if (int.TryParse(answerCountRequest.downloadHandler.text, out answerCount)) playerData.AnswerCount = answerCount;

            Epoch.SingleEpoch suitableEpoch = Epoch.Epochs.Where(epoch => epoch.townsRequired <= playerData.NrOfLands && epoch.correctAnswerRatioRequired <= playerData.CorrectAnswerRatio).OrderByDescending(epoch => epoch.correctAnswerRatioRequired).FirstOrDefault();
            playerData.Epoch = suitableEpoch.name;

            AllPlayersMapData.Add(playerData);

            //przypisanie epok landom
            foreach (MapDataOfAPlayer playerMapData in AllPlayersMapData)
            {
                int ownerID = playerMapData.ID;
                string epoch = playerMapData.Epoch;
                for (int i = 0; i < tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < tiles.GetLength(1); j++)
                    {
                        Land landScript = tiles[i, j].GetComponent<Land>();
                        if (landScript.OwnerID == ownerID) landScript.Epoch = epoch;
                    }
                }
            }

            //przypisanie odpowiednich sprite'ow miastom
            foreach (GameObject tile in tiles)
            {
                for (int i = 0; i < Epoch.Epochs.Length; i++)
                {
                    if (tile.GetComponent<Land>().Epoch == Epoch.Epochs[i].name)
                    {
                        tile.GetComponent<Image>().sprite = epochTownSprite[i];
                        break;
                    }
                }
            }
        }
    }

    public IEnumerator GetPlayerIds()
    {
        UnityWebRequest www = UnityWebRequest.Get($"https://localhost:7060/api/getplayerids");
        www.SendWebRequest();
        while (!www.isDone) yield return null;
        string response = www.downloadHandler.text;
        playerIds = JsonConvert.DeserializeObject<List<int>>(response);
    }
}