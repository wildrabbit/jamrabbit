using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Pair<T>
{
    public T first;
    public T second;

	public Pair(T first, T second)
    {
        this.first = first;
        this.second = second;
    }

    public void Set(T first, T second)
    {
        this.first = first;
        this.second = second;
    }
}