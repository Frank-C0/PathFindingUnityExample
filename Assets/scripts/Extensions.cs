using System;
using System.Collections.Generic;

// Extensiones genéricas útiles para colecciones.
public static class Extensions
{
    private static Random rng = new Random();

    // Mezcla aleatoriamente una lista genérica utilizando el algoritmo Fisher-Yates.
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