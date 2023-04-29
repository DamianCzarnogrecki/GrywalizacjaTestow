using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float time = 0f;
    public int passedSeconds = 0;

    void Update()
    {
        time += Time.deltaTime;

        if (time > 1f)
        {
            passedSeconds += 1;
            time = 0f;
        }
    }
}
