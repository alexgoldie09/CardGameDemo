using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = System.Random;

public static class ShufflingExtension
{
    /// <summary>
    /// A static instance of the Random class to be used for shuffling.
    /// This ensures that the same random sequence is not generated each time the method is called.
    /// </summary>
    private static Random rng = new Random();

    /// <summary>
    /// Shuffles the elements of the list in place using the Fisher-Yates algorithm.
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
