using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer TimerInstance { get; private set; }
    private float time = 0f;
    public int passedSeconds = 0;

    private void Awake() 
    { 
        //singleton: usuniecie ewentualnych innych instancji
        if (TimerInstance != null && TimerInstance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            TimerInstance = this; 
        } 
    }

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
