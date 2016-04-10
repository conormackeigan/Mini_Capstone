using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    // Constructor
    public PriorityQueue()
    {
        data = new List<KeyValuePair<int, T>>();
    }

    // struct data
    private List<KeyValuePair<int, T>> data; // int = priority

    public void Add(T item, int priority)
    {
        KeyValuePair<int, T> entry = new KeyValuePair<int, T>(priority, item);
        data.Add(entry);

        // resort list on add 
        Sort();   
    }

    public T delete_min()
    {
        T min = data[0].Value;
        data.RemoveAt(0);
        return min;
    }

    public T delete_max()
    {
        T max = data[data.Count - 1].Value;
        data.RemoveAt(data.Count - 1);
        return max;
    }

    public void Sort()
    { // demi-bubble sort
        for (int i = data.Count - 1; i > 0; i--)
        { // only do one passthru because for our needs items are only ever added one at a time,
          // and this sort is called upon every entry. update if needed 
            if (data[i].Key < data[i - 1].Key)
            {
                Swap(data[i], data[i - 1]);
            }
        }
    }

    public bool Empty()
    {
        if (data.Count < 1)
        {
            return true;
        }
        return false;
    }

    public void AddOrUpdate(T value, int priority)
    {
        if (!contains(value))
        {
            Add(value, priority);
        }
        else
        {
            changePriority(value, priority);
        }
    }

    public bool contains(T item)
    {
        foreach (KeyValuePair<int, T> d in data)
        {
            if (d.Value.Equals(item))
            {
                return true;
            }
        }
        return false;
    }

    public void changePriority(T value, int priority)
    {
        foreach (KeyValuePair<int, T> d in data)
        {
            if (d.Value.Equals(value))
            {
                KeyValuePair<int, T> entry = new KeyValuePair<int, T>(priority, d.Value);
                data.Remove(d);
                data.Add(entry);

                break;
            }
        }
    }

    public KeyValuePair<int, T> Swap(KeyValuePair<int, T> item1, KeyValuePair<int, T> item2)
    {
        KeyValuePair<int, T> tmp = item1;
        item1 = item2;
        item2 = tmp;

        return item2;
    }

    public T front()
    {
        if (data.Count > 0)
        {
            return data[0].Value;
        }
        else
        {
            return default(T); // empty list cannot return an item
        }
    }

    public T back()
    {
        if (data.Count > 0)
        {
            return data[data.Count - 1].Value;
        }
        else
        {
            return default(T);
        }
    }

    public int frontPriority()
    {
        if (data.Count > 0)
        {
            return data[0].Key;
        }
        else
        {
            return (int)IntConstants.INVALID;
        }
    }

    public void Clear()
    {
        data.Clear();
    }
}
