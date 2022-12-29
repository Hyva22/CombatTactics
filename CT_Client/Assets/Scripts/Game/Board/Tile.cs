using UnityEngine;

public enum TileType
{
    None,
    Grass,
    Forest,
    Mountain,
    Water,
    Beach,
    Street,
    City,
}

public class Tile : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private TileType type;

    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {

    }
}
