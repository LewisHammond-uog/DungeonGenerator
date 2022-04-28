using UnityEngine;

public class TileMap3D : MonoBehaviour
{
    protected Vector2Int mapSize;
    protected GameObject[,] map;
    
    public void Init(Vector2Int size)
    {
        mapSize = size;
        map = new GameObject[size.x, size.y];
    }

    /// <summary>
    /// Spawn a tile in a given position in the array
    /// </summary>
    /// <param name="pos">Position to spawn at</param>
    /// <param name="tilePrefab">Prefab to spawn</param>
    public void SpawnTile(Vector2Int pos, GameObject tilePrefab)
    {
        if (!IsInTileMap(pos))
        {
            Debug.LogError("Tile not in map bounds");
            return;
        }
        Vector3 posV3 = new Vector3(pos.x, pos.y, 0);
        GameObject tile = Instantiate(tilePrefab, posV3, Quaternion.Euler(90, 0, 0));

        map[pos.x, pos.y] = tile;
    }

    /// <summary>
    /// Set a tile, already spawned to be in the array
    /// Does NOT spawn tiles or move them to the correct position
    /// </summary>
    /// <param name="tileMapPos"></param>
    /// <param name="tile"></param>
    private void SetTileInMap(Vector2Int tileMapPos, GameObject tile)
    {
        if (!IsInTileMap(tileMapPos))
        {
            Debug.LogError("Tile not in map bounds");
            return;
        }

        //Check the object that we are providing is in the correct location
        Vector3 objWorldPos = tile.gameObject.transform.position;
        if (objWorldPos.x - tileMapPos.x > float.Epsilon ||
            objWorldPos.y - tileMapPos.y > float.Epsilon)
        {
            Debug.LogWarning("Setting tile in TileMap to a different position than in the world" +
                             $"{objWorldPos} vs {tileMapPos}");
        }

        map[tileMapPos.x, tileMapPos.y] = tile;
    }

    private bool IsInTileMap(Vector2Int pos)
    {
        const int xDimension = 0;
        const int yDimension = 1;
        
        return pos.x > 0 && pos.x < map.GetLength(xDimension) 
                         && pos.y > 0 && pos.y < map.GetLength(yDimension);
    }
}