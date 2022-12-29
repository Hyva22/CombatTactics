using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private int width, height;
    [SerializeField] private GridLayoutGroup tileContainer;
    [SerializeField] private SEnumeratorDictionary<TileType, Tile> typeMap;
    [SerializeField] private SDictionary<Vector2, Tile> tiles;

    public void OnAfterDeserialize()
    {
        tiles.ForEach(pair =>
        {
            pair.value.gameObject.SetActive(pair.key.x < width && pair.key.y < height);
        });
    }

    public void OnBeforeSerialize(){}
}
