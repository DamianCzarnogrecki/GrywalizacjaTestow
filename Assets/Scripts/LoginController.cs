using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine.UI;
using System;
using System.Text;

public class LoginController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nicknameField, passwordField;
    [SerializeField]
    private TextMeshProUGUI errorText;
    private string nickname, password;
    public int playerid = 0;
    [SerializeField]
    private GameObject loginPanel, loginButton, logoutButton;
    public PlayerDataController playerDataController;

    public void Login()
    {
        nickname = nicknameField.text;
        password = passwordField.text;

        if (
            nickname.Length < 1
            || nickname.Length > 32
            || Regex.IsMatch(nickname, "^[A-Za-z0-9_]", RegexOptions.IgnoreCase) == false
            || password.Length < 1
            || password.Length > 32
            || Regex.IsMatch(password, "^[A-Za-z0-9_]", RegexOptions.IgnoreCase) == false
        )
        {
            errorText.text = "PODANO NIEPRAWIDŁOWE DANE";
        }
        else
        {
            StartCoroutine(SendForm(nickname, password));
        }

        IEnumerator SendForm(string nickname, string password)
        {
            string url = "https://localhost:7060/api/login";
            int returnedID = 0;

            using (HttpClient client = new HttpClient())
            {
                var json = $"{{\"login\": \"{nickname}\", \"password\": \"{password}\"}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var task = client.PostAsync(url, content);
                yield return new WaitUntil(() => task.IsCompleted);
                
                if (task.Result.IsSuccessStatusCode)
                {
                    var response = task.Result.Content.ReadAsStringAsync().Result;
                    Int32.TryParse(response, out returnedID);

                    errorText.text = returnedID == 0 ? "BŁĄD LOGOWANIA" : "";
                }
                else errorText.text = "BŁĄD LOGOWANIA";
                playerDataController.playerid = returnedID;
                
            }
        }
    }
}