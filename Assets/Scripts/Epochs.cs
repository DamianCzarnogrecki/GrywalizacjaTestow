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
        new SingleEpoch("brązu", 2, 37.5f),
        new SingleEpoch("żelaza", 4, 45f),
        new SingleEpoch("średniowiecza", 6, 52.5f),
        new SingleEpoch("renesansu", 8, 60f),
        new SingleEpoch("oświecenia", 10, 67.5f),
        new SingleEpoch("przemysłowa", 12, 75f),
        new SingleEpoch("elektryczności", 14, 80f),
        new SingleEpoch("komputerów", 16, 85f),
        new SingleEpoch("danych", 18, 90f),
        new SingleEpoch("kosmiczna", 20, 93.75f),
        new SingleEpoch("transcendencji", 22, 97.5f),
    };
}