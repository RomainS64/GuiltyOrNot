using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomizeId : MonoBehaviour
{
    
    void Awake()
    {
        List<PhotoSetter> photoSetters = GetComponentsInChildren<PhotoSetter>(true).ToList();
        Shuffle(photoSetters);
        for (int i = 0; i < photoSetters.Count; i++)
        {
            photoSetters[i].SetPlayerId(i);
        }
    }
    static public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
