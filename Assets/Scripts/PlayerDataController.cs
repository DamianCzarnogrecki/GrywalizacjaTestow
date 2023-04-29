using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataController : MonoBehaviour
{
    public static PlayerDataController PlayerDataControllerInstance { get; private set; }
    public int playerid = 0;
    public LoginController loginController;

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
}