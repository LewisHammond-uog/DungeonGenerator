using System;
using DefaultNamespace;
using UnityEngine;

public class TileMap3D : MonoBehaviour
{
    protected Vector2Int mapSize;
    protected GameObject[,] map;
    public GameObject[,] TileMap => map;

    [SerializeField] private bool drawDebugTiles = false;
    
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
        Vector3 posV3 = new Vector3(pos.x, 0, pos.y);
        GameObject tile = Instantiate(tilePrefab, posV3, Quaternion.identity);

        SetTileInMap(pos, tile);
    }

    public void InsertRoom(BSPTreeNode node, Vector2Int roomCenterSpawnPos, GameObject roomPrefab)
    {
        Vector3 posV3 = new Vector3(roomCenterSpawnPos.x, 0, roomCenterSpawnPos.y);
        GameObject room = Instantiate(roomPrefab, posV3,  Quaternion.identity);
        
        //Get tilemap comp from room
        if (!room.TryGetComponent(out RoomTileMap roomMapComp))
        {
            Debug.LogError($"Cannot spawn room ({roomPrefab.name}) in tilemap, no {nameof(RoomTileMap)} Component");
            Destroy(room);
            return;
        }
        
        //Get the room tilemap setup
        roomMapComp.FillMapWithObjects();
        
        //Get room center - move room to be centered on grid
        Vector2Int roomCenter = roomMapComp.GetRoomCenterOnGrid();
        room.transform.position -= new Vector3(roomCenter.x, 0, roomCenter.y);
        
        //Set node
        Vector2Int roomSize = roomMapComp.mapSize;
        Vector2Int roomWorldPos = new Vector2Int(Mathf.RoundToInt(room.transform.position.x),
            Mathf.RoundToInt(room.transform.position.z));
        node.room = new RectInt(roomWorldPos, roomSize);
        
        //Add to tilemap
        node.roomTileMapComp = roomMapComp;
        GameObject[,] roomMap = roomMapComp.map;
        for (int x = 0; x < roomMapComp.mapSize.x; x++)
        {
            for (int y = 0; y < roomMapComp.mapSize.y; y++)
            {
                Vector2Int mainMapPos = new Vector2Int(roomCenterSpawnPos.x + x - roomCenter.x,
                    roomCenterSpawnPos.y + y - roomCenter.y);
                GameObject floorObj = roomMap[x, y];
                
                if (!IsInTileMap(mainMapPos))
                {
                    Debug.LogError("Tile not in map bounds");
                    Destroy(room);
                    return;
                }
                
                SetTileInMap(mainMapPos, floorObj);
            }
        }
    }

    /// <summary>
    /// Get tile at given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GameObject GetTile(Vector2Int pos)
    {
        return !IsInTileMap(pos) ? null : map[pos.x, pos.y];
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
        
        if (tile)
        {
            //Check the object that we are providing is in the correct location
            Vector3 objWorldPos = tile.gameObject.transform.position;
            float diff = Vector3.Distance(objWorldPos, new Vector3(tileMapPos.x, 0, tileMapPos.y));
            const float reasonableDist = 0.1f; //Epsilon is too small for unity's FP differences
            if (diff > reasonableDist)
            {
                Debug.LogWarning("Setting tile in TileMap to a different position than in the world" +
                                 $"{objWorldPos} vs {tileMapPos}");
            }
        }

        RemoveTile(tileMapPos);
        map[tileMapPos.x, tileMapPos.y] = tile;
    }

    /// <summary>
    /// Remove tile at position
    /// </summary>
    /// <param name="pos"></param>
    private void RemoveTile(Vector2Int pos)
    {
        if (!IsInTileMap(pos))
        {
            Debug.LogError("Tile not in map bounds");
            return;
        }

        GameObject objInTile = map[pos.x, pos.y];
        if (objInTile != null)
        {
            Destroy(objInTile);
        }

        map[pos.x, pos.y] = null;
    }

    private bool IsInTileMap(Vector2Int pos)
    {
        const int xDimension = 0;
        const int yDimension = 1;
        
        return pos.x > 0 && pos.x < map.GetLength(xDimension) 
                         && pos.y > 0 && pos.y < map.GetLength(yDimension);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (drawDebugTiles)
        {
            DrawDebugTiles();
        }
    }

    private void DrawDebugTiles()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Color nullColour = new Color(1f, 0f, 0f, 0.25f);
                Gizmos.color = map[x, y] != null ? Color.green : nullColour;
                Gizmos.DrawCube(new Vector3(x,0,y), Vector3.one);
            }
        }
        
        Gizmos.color = Color.white;
    }

#endif
}