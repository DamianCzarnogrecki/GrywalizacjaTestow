using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Epoch
{
    public struct SingleEpoch
    {
        public string name;
        public int townsRequired;
        public float correctAnswerRatioRequired;

        public SingleEpoch(string name, int townsRequired, float correctAnswerRatioRequired)
        {
            this.name = name;
            this.townsRequired = townsRequired;
            this.correctAnswerRatioRequired = correctAnswerRatioRequired;
        }
    }

    public static SingleEpoch[] Epochs = new SingleEpoch[]
    {
        new SingleEpoch("neolitu", 0, 0f),
        new SingleEpoch("brazu", 2, 37.5f),
        new SingleEpoch("zelaza", 4, 45f),
        new SingleEpoch("sredniowiecza", 6, 52.5f),
        new SingleEpoch("renesansu", 8, 60f),
        new SingleEpoch("oswiecenia", 10, 67.5f),
        new SingleEpoch("przemyslowa", 12, 75f),
        new SingleEpoch("elektrycznosci", 14, 80f),
        new SingleEpoch("komputerow", 16, 85f),
        new SingleEpoch("danych", 18, 90f),
        new SingleEpoch("kosmiczna", 20, 93.75f),
        new SingleEpoch("transcendencji", 22, 97.5f),
    };
}