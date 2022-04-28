using UnityEngine;

public class TileMap3D : MonoBehaviour
{
    private Vector2Int mapSize;
    private GameObject[,] map;
    
    public void Init(Vector2Int size)
    {
        mapSize = size;
        map = new GameObject[size.x, size.y];
    }

    public void SetTile(Vector2Int pos, GameObject tile)
    {
        if (!IsInTileMap(pos))
        {
            Debug.LogError("Tile not in map bounds");
            return;
        }
        Vector3 posV3 = new Vector3(pos.x, pos.y, 0);
        Instantiate(tile, posV3, Quaternion.Euler(90, 0, 0));
    }

    private bool IsInTileMap(Vector2Int pos)
    {
        const int xDimension = 0;
        const int yDimension = 1;
        
        return pos.x > 0 && pos.x < map.GetLength(xDimension) 
                         && pos.y > 0 && pos.y < map.GetLength(yDimension);
    }
}