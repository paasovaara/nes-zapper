using System;
using System.Collections.Generic;

public static class ExtensionUtilities {
    private static Random rand = new Random();

    //Extension method for List based on Fisher-Yates shuffle
    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
