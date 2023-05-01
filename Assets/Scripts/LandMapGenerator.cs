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

public class LandMapGenerator : MonoBehaviour
{
    public int Rows = 10;
    public int Columns = 10;
    public float TileWidth = 30f;
    public float TileHeight = 30f;
    public GameObject TilePrefab;
    public Vector2 TilesOffset;
    public List<LandOwner> LandOwners;
    private GameObject[,] tiles;

    void Start()
    {
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


 
}