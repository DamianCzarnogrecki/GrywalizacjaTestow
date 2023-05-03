using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWiper : MonoBehaviour
{
    public TextMeshProUGUI textToWipe;

    public void WipeText()
    {
        textToWipe.text = "";
    }
}
