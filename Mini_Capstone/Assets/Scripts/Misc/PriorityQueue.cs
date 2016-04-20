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
    public List<KeyValuePair<int, T>> data; // int = priority

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
        for (int i = data.Count - 1; i >= 0; i--)
        { // only do one passthru because for our needs items are only ever added one at a time,
          // and this sort is called upon every entry. update if needed 

            if (i != 0)
            {
                if (data[i].Key < data[i - 1].Key)
                {
                    Swap(i, i - 1);
                }
            }
            else if (data.Count > 1)
            {
                if (data[i].Key > data[i + 1].Key)
                {
                    Swap(i, i + 1);
                }
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


    public int Count()
    {
        return data.Count;
    }

    public T At(int index)
    {
        if (data.Count - index < 0)
        {
            return default(T);
        }
        else
        {
            return data[index].Value;
        }
    }

    public int priorityAt(int index)
    {
        if (data.Count - index < 0)
        {
            return (int)IntConstants.INVALID;
        }
        else
        {
            return data[index].Key;
        }
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

    // same as above function but only updates if the new value has a higher priority value
    public void AddOrUpdateIfHigher(T value, int priority)
    {
        if (!contains(value))
        {
            Add(value, priority);
        }
        else if (data[indexOf(value)].Key < priority)
        {
            changePriority(value, priority);
        }
    }

    // same as above but updates if priority value is lower, not higher
    public void AddOrUpdateIfLower(T value, int priority)
    {
        if (!contains(value))
        {
            Add(value, priority);
        }
        else if (data[indexOf(value)].Key > priority)
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

    // gets index of a specified item
    public int indexOf(T item)
    {
        int counter = 0;

        foreach(KeyValuePair<int, T> d in data)
        {
            if (d.Value.Equals(item))
            {
                return counter;
            }

            counter++;
        }

        return (int)IntConstants.INVALID;
    }

    public void changePriority(T value, int priority)
    {
        foreach (KeyValuePair<int, T> d in data)
        {
            if (d.Value.Equals(value))
            {
                data.Remove(d);
                Add(value, priority);

                break;
            }
        }
    }

    public void Swap(int index1, int index2)
    {
        KeyValuePair<int, T> tmp = new KeyValuePair<int, T>(data[index1].Key, data[index1].Value);
        data[index1] = new KeyValuePair<int, T>(data[index2].Key, data[index2].Value);
        data[index2] = tmp;
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

    public int backPriority()
    {
        if (data.Count > 0)
        {
            return data[data.Count - 1].Key;
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
