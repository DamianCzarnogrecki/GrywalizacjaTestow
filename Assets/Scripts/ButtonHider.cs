using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonHider : MonoBehaviour
{
    public void HideTheButton()
    {
        gameObject.GetComponent<Image>().enabled = false;
        gameObject.GetComponent<Button>().enabled = false;
        gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().enabled = false;
    }
}
