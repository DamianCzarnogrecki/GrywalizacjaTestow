using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerController : MonoBehaviour
{
    public int id;

    public void CheckAnswer()
    {
        GameObject.Find("Question").GetComponent<QuestionController>().CheckAnswer(id);
    }
}
