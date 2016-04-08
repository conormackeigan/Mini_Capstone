using UnityEngine;
using System.Collections;


public class Pair<T1, T2>
{
    T1 first;
    T2 second;

    public Pair(T1 f, T2 s)
    {
        first = f;
        second = s;
    }
}
