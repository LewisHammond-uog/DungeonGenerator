using System;
using UnityEngine;

[Serializable]
public struct RoomInfo
{
    public GameObject roomPrefab;
    public Vector2Int size;
    
    public int GetMaxDimension()
    {
        return size.x > size.y ? size.x : size.y;
    }
}
