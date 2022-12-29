using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SDictionary<K,V>
{
    [SerializeField]
    public List<Pair<K, V>> elements;

    public SDictionary()
    {
        elements = new List<Pair<K, V>>();
    }

    public List<K> GetKeys()
    {
        List<K> keys = new List<K>(elements.Count);
        elements.ForEach(element => keys.Add(element.key));
        return keys;
    }

    public List<V> GetValues()
    {
        List<V> keys = new List<V>(elements.Count);
        elements.ForEach(element => keys.Add(element.value));
        return keys;
    }

    public bool Contains(K key)
    {
        return TryGetValue(key, out _);
    }

    public void Add(K key, V value)
    {
        elements.Add(new Pair<K, V>(key, value));
    }

    public void Remove(K key)
    {
        bool found = TryGetPair(key, out Pair<K, V> pair);
        if (!found)
            return;
        elements.Remove(pair);
    }

    public Pair<K,V> this[int index]
    {
        get => elements[index];
        set => elements[index] = value;
    }

    public int Count
    {
        get => elements.Count;
    }

    public void ForEach(Action<Pair<K, V>> action)
    {
        elements.ForEach(action);
    }

    public Pair<K, V> Find(Predicate<Pair<K,V>> predicate)
    {
        return elements.Find(predicate);
    }

    public SDictionary<K, V> FindAll(Predicate<Pair<K,V>> predicate)
    {
        SDictionary<K, V> results = new();
        elements.ForEach(pair =>
        {
            if (predicate(pair))
                results.elements.Add(pair);
        });
        return results;
    }

    public void SortByKey()
    {
        Comparer<K> comparer = Comparer<K>.Default;
        elements.Sort(delegate (Pair<K, V> a, Pair<K, V> b)
        {
            return comparer.Compare(a.key, b.key);
        });
    }

    public void SortByValue()
    {
        Comparer<V> comparer = Comparer<V>.Default;
        elements.Sort(delegate (Pair<K, V> a, Pair<K, V> b)
        {
            return comparer.Compare(a.value, b.value);
        });
    }

    public bool TrySetValue(K key, V value)
    {
        bool found = TryGetPair(key, out Pair<K, V> pair);
        if (!found)
            return false;

        pair.value = value;
        return true;
    }

    public bool TryGetPair(K key, out Pair<K,V> value)
    {
        Comparer<K> defComp = Comparer<K>.Default;

        foreach (Pair<K, V> element in elements)
        {
            if (defComp.Compare(element.key, key) == 0)
            {
                value = element;
                return true;
            }
        }
        value = default;
        return false;
    }

    public bool TryGetValue(K key, out V value)
    {
        bool found = TryGetPair(key, out Pair<K, V> pair);
        value = pair == null ? default : pair.value;
        return found;
    }
}
