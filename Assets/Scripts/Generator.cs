using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    private BSPTree tree;

    [SerializeField] private int numberOfItterations = 10;
    [SerializeField] private int corridorThickness = 2;

    [SerializeField] private Vector2Int dungeonSize;
    [SerializeField] private Vector2Int minRoomSize;

    [SerializeField] private RoomInfo[] rooms;

    [SerializeField] private GameObject corridor;

    #region Tiles
    [HideInInspector] public GameObject tlTile;
    [HideInInspector] public GameObject tmTile;
    [HideInInspector] public GameObject trTile;
    [HideInInspector] public GameObject mlTile;
    [HideInInspector] public GameObject mmTile;
    [HideInInspector] public GameObject mrTile;
    [HideInInspector] public GameObject blTile;
    [HideInInspector] public GameObject bmTile;
    [HideInInspector] public GameObject brTile;
    #endregion

    private TileMap3D map;
    public TileMap3D TileMap => map;
    private DijkstraMap distFromStartMap;
    private Pathfinder shortestRouteFinder;

    [SerializeField] private Image testImage;
    [SerializeField] private Gradient distGradient;
    private Texture2D startMapTexture;
    
    public void Init()
    {
        map = gameObject.AddComponent<TileMap3D>();
        map.Init(dungeonSize);
        
        //Sprite for dist from start map
        startMapTexture = new Texture2D(dungeonSize.x, dungeonSize.y);
        Sprite distanceMapSprite = Sprite.Create(startMapTexture, new Rect(0,0,dungeonSize.x,dungeonSize.y), new Vector2(0,0));
        testImage.sprite = distanceMapSprite;
        testImage.rectTransform.sizeDelta = dungeonSize;
        testImage.transform.parent.position = new Vector3(dungeonSize.x / 2 - 0.5f, 1, dungeonSize.y / 2 - 4 + 0.5f);
        
        shortestRouteFinder = new Pathfinder();
    }

    public void GenerateSpace()
    {
        tree = new BSPTree(numberOfItterations, dungeonSize, minRoomSize);
        GenerateRooms();
    }

    private void GenerateRooms()
    {
        List<BSPTreeNode> leafNodes = new List<BSPTreeNode>();
        tree.GetLeafNodes(ref leafNodes);

        foreach (BSPTreeNode node in leafNodes)
        {
            Vector2Int spawnPos = Vector2Int.RoundToInt(node.container.center);
            map.InsertRoom(node, spawnPos, RoomUtils.GetRoomPrefabForNode(node, rooms));
        }
        
    }

    public IEnumerator GenerateCorridors(float delayPerCorridor, float delayPerTile)
    {
        if (tree == null || tree.RootNode == null)
        {
            yield break;
        }
        
        //Optimization create the delays once, not every frame
        WaitForSeconds corridorDelay = new WaitForSeconds(delayPerCorridor);
        WaitForSeconds tileDelay = new WaitForSeconds(delayPerTile);
        yield return GenerateCorridorsNode(tree.RootNode, corridorDelay, tileDelay);
    }

    private IEnumerator GenerateCorridorsNode(BSPTreeNode node, WaitForSeconds delayPerCorridor, WaitForSeconds delayPerTile)
    {
        if (!node.IsInternal)
        {
            yield break;
        }
        
        RectInt leftContainer = node.left.container;
        RectInt rightContainer = node.right.container;
            
        Vector2 leftCenter = leftContainer.center;
        Vector2 rightCenter = rightContainer.center;
        Vector2 direction = (rightCenter - leftCenter).normalized;

        while (Vector2.Distance(leftCenter, rightCenter) > 0.51f)
        {
            if (direction.Equals(Vector2.right))
            {
                for (int i = 0; i < corridorThickness; i++)
                {
                    map.SpawnTile(new Vector2Int((int) leftCenter.x, (int) leftCenter.y + i), corridor);
                }
                
                yield return delayPerTile;

            }else if (direction.Equals(Vector2.up))
            {
                for (int i = 0; i < corridorThickness; i++)
                {
                    map.SpawnTile(new Vector2Int((int) leftCenter.x + i, (int) leftCenter.y), corridor);
                }
                
                yield return delayPerTile;
            }
                
            leftCenter.x += direction.x;
            leftCenter.y += direction.y;
        }

        if (node.left != null)
        {
            yield return GenerateCorridorsNode(node.left, delayPerCorridor, delayPerTile);
            yield return delayPerCorridor;
        }

        if (node.right != null)
        {
            yield return GenerateCorridorsNode(node.right, delayPerCorridor, delayPerTile);
            yield return delayPerCorridor;
        }
    }

    /// <summary>
    /// Get the correct tile in the tile map to use based on our neighbours
    /// </summary>
    private GameObject GetTileFromNeighbours(int x, int y)
    {
        GameObject mmGridTile = map.GetTile (new Vector2Int (x,   y));
        if (mmGridTile == null) return null; //repaint anything that does have a 

        GameObject blGridTile = map.GetTile (new Vector2Int (x-1, y-1));
        GameObject bmGridTile = map.GetTile (new Vector2Int (x,   y-1));
        GameObject brGridTile = map.GetTile (new Vector2Int (x+1, y-1));

        GameObject mlGridTile = map.GetTile (new Vector2Int (x-1, y));
        GameObject mrGridTile = map.GetTile (new Vector2Int (x+1, y));

        GameObject tlGridTile = map.GetTile (new Vector2Int (x-1, y+1));
        GameObject tmGridTile = map.GetTile (new Vector2Int (x,   y+1));
        GameObject trGridTile = map.GetTile (new Vector2Int (x+1, y+1));

        // we have 8 + 1 cases
		
        // left
        if (mlGridTile == null && tmGridTile == null) return tlTile;
        if (mlGridTile == null && tmGridTile != null && bmGridTile != null) return mlTile;
        if (mlGridTile == null && bmGridTile == null && tmGridTile != null) return blTile;
		
        // middle
        if (mlGridTile != null && tmGridTile == null && mrGridTile != null) return tmTile;
        if (mlGridTile != null && bmGridTile == null && mrGridTile != null) return bmTile;
		
        // right
        if (mlGridTile != null && tmGridTile == null && mrGridTile == null) return trTile;
        if (tmGridTile != null && bmGridTile != null && mrGridTile == null) return mrTile;
        if (tmGridTile != null && bmGridTile == null && mrGridTile == null) return brTile;

        return mmTile; // default case
    }

    public void PaintTiles()
    {
        for (int i = 0; i < dungeonSize.x; i++) {
            for (int j = 0; j < dungeonSize.y; j++) {
                GameObject tile = GetTileFromNeighbours (i, j);
                if (tile != null) {
                    map.SpawnTile(new Vector2Int(i,j), tile);
                }
            }
        }
    }

    public IEnumerator GenerateDistFromStartMap()
    {
        tree.ChooseStartAndEndNode();
        
        distFromStartMap = new DijkstraMap();
        distFromStartMap.Initialize(new Vector2Int((int)tree.StartRoom.container.center.x, (int)tree.StartRoom.container.center.y)
            , map.TileMap);
        
        while (!distFromStartMap.IsFinished)
        {
            distFromStartMap.Update();
            distFromStartMap.GetMapAsTexture(ref startMapTexture, distGradient);
            yield return null;
        }
    }

    public IEnumerator DrawHotPath(float delayPerCube)
    {
        Vector2Int destination =
            new Vector2Int((int) tree.EndRoom.container.center.x, (int) tree.EndRoom.container.center.y);
        
        shortestRouteFinder.Initialize(distFromStartMap, destination);

        while (!shortestRouteFinder.IsFinished)
        {
            shortestRouteFinder.Update();
            shortestRouteFinder.AddPathToTexture(ref startMapTexture);
            MarkHotPathRooms(shortestRouteFinder.route);
            DoHotPathColour();
            yield return new WaitForSeconds(delayPerCube);
        }
    }

    private void MarkHotPathRooms(List<Vector2Int> route)
    {
        List<BSPTreeNode> leafNodes = new List<BSPTreeNode>();
        tree.GetLeafNodes(ref leafNodes);
        foreach (BSPTreeNode node in leafNodes)
        {
            node.DetermineIsOnHotPath(route);
        }
    }

    private void DoHotPathColour()
    {
        List<BSPTreeNode> leafNodes = new List<BSPTreeNode>();
        tree.GetLeafNodes(ref leafNodes);
        foreach (BSPTreeNode node in leafNodes)
        {
            if (node.IsOnHotPath)
            {
                Vector2Int roomCenterSpawnPos = Vector2Int.RoundToInt(node.room.center);
                Vector2Int roomCenter = node.roomTileMapComp.GetRoomCenterOnGrid();
                
                for (int x = 0; x < node.room.size.x; x++)
                {
                    for (int y = 0; y < node.room.size.y; y++)
                    {

                        Vector2Int mainMapPos = new Vector2Int(roomCenterSpawnPos.x + x - roomCenter.x,
                            roomCenterSpawnPos.y + y - roomCenter.y);

                        if (map.TileMap[mainMapPos.x, mainMapPos.y] != null)
                        {
                            startMapTexture.SetPixel(mainMapPos.x, mainMapPos.y, Color.magenta);
                        }
                        
                    }
                }
            }
        }
        startMapTexture.Apply();
    }


#if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        DebugDrawBsp();
    }

    private void DebugDrawBsp () {
        if (tree == null) return; // hasn't been generated yet
        if (tree.RootNode == null) return; // hasn't been generated yet

        List<BSPTreeNode> leafNodes = new List<BSPTreeNode>();
        tree.GetLeafNodes(ref leafNodes);

        foreach (BSPTreeNode node in leafNodes)
        {
            DebugDrawBspNode (node); 
        }
    }

    private void DebugDrawBspNode (BSPTreeNode node) {
        // Container
        Gizmos.color = Color.green;
        // top
        Gizmos.DrawLine (new Vector3 (node.container.x, 0, node.container.y), new Vector3Int (node.container.xMax, 0, node.container.y));
        // right
        Gizmos.DrawLine (new Vector3 (node.container.xMax, 0, node.container.y), new Vector3Int (node.container.xMax, 0, node.container.yMax));
        // bottom
        Gizmos.DrawLine (new Vector3 (node.container.x, 0, node.container.yMax), new Vector3Int (node.container.xMax, 0, node.container.yMax));
        // left
        Gizmos.DrawLine (new Vector3 (node.container.x, 0, node.container.y), new Vector3Int (node.container.x, 0, node.container.yMax));

        // children
        if (node.left != null) DebugDrawBspNode (node.left);
        if (node.right != null) DebugDrawBspNode (node.right);
    }
#endif
    
}
