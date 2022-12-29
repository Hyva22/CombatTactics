using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SEnumeratorDictionary<K, V> : SDictionary<K, V>, ISerializationCallbackReceiver where K : Enum
{
    public SEnumeratorDictionary() : base()
    {
        HashKeys();
    }

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        HashKeys();
    }

    public void HashKeys()
    {
        Array values = Enum.GetValues(typeof(K));
        int length = values.Length;
        List<Pair<K, V>> newElements = new(new Pair<K, V>[length]);

        for (int i = 0; i < length; i++)
        {
            if (newElements[i] == null)
                newElements[i] = new Pair<K, V>((K)values.GetValue(i), default);
        }

        elements.ForEach(element =>
        {
            try
            {
                newElements[(int)(object)element.key] = element;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to assign old value after Hashing keys. Error: {e.Message}");
            }
        });

        elements = new List<Pair<K, V>>(newElements);
    }

    public new V this[int index]
    {
        get => elements[index].value;
        set => elements[index].value = value;
    }

    public V this[K index]
    {
        get => this[(int)(object)index];
        set => this[(int)(object)index] = value;
    }
}
