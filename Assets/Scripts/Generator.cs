using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    public BSPTree tree;

    private int numberOfItterations = 10;
    private int corridorThickness = 2;

    private Vector2Int dungeonSize;
    private Vector2Int minRoomSize;

    [SerializeField] private RoomInfo[] rooms;
    [SerializeField] private GameObject corridor;
    private List<GameObject> spawnedRooms;

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

    [SerializeField] public GameObject tSoloTile;
    [SerializeField] public GameObject rSoloTile;
    [SerializeField] public GameObject lSoloTile;
    [SerializeField] public GameObject bSoloTile;
    #endregion

    private TileMap3D map;
    public TileMap3D TileMap => map;
    private DijkstraMap distFromStartMap;
    private Pathfinder shortestRouteFinder;

    [SerializeField] private Image dMapImage;
    [SerializeField] private Gradient distGradient;
    private ProtectedTexture2D startMapTexture;
    
    public void Init(Vector2Int dungeonBounds, Vector2Int minCellSize, int corridorThickness, int itterations)
    {
        dungeonSize = dungeonBounds;
        minRoomSize = minCellSize;
        numberOfItterations = itterations;
        this.corridorThickness = corridorThickness;
        
        map = gameObject.AddComponent<TileMap3D>();
        map.Init(dungeonSize);
        
        //Sprite for dist from start map
        startMapTexture = new ProtectedTexture2D(dungeonSize.x, dungeonSize.y);
        Sprite distanceMapSprite = Sprite.Create(startMapTexture.texture, new Rect(0,0,dungeonSize.x,dungeonSize.y), new Vector2(0,0));
        dMapImage.sprite = distanceMapSprite;
        dMapImage.rectTransform.sizeDelta = dungeonSize;
        dMapImage.transform.parent.position = new Vector3(dungeonSize.x / 2 - 0.5f, 1, dungeonSize.y / 2 - 4 + 0.5f);
        
        shortestRouteFinder = new Pathfinder();

        spawnedRooms = new List<GameObject>();
    }

    public void ResetGenerator()
    {
        tree = null;
        Destroy(map);
        startMapTexture = null;
        shortestRouteFinder = null;
    }

    /// <summary>
    /// Generate the space for the rooms to go in
    /// </summary>
    public void GenerateSpace()
    {
        tree = new BSPTree(numberOfItterations, dungeonSize, minRoomSize);
    }

    /// <summary>
    /// Generate the rooms in the tree space
    /// </summary>
    public void GenerateRooms()
    {
        List<BSPTreeNode> leafNodes = new List<BSPTreeNode>();
        tree.GetLeafNodes(ref leafNodes);

        foreach (BSPTreeNode node in leafNodes)
        {
            Vector2Int spawnPos = Vector2Int.RoundToInt(node.container.center);
            map.InsertRoom(node, spawnPos, RoomUtils.GetRoomPrefabForNode(node, rooms));
        }
        
    }

    /// <summary>
    /// Generate corridors for the entire tree
    /// </summary>
    /// <param name="delayPerCorridor"></param>
    /// <param name="delayPerTile"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Generate Corridors between 2 nodes
    /// </summary>
    /// <param name="node"></param>
    /// <param name="delayPerCorridor"></param>
    /// <param name="delayPerTile"></param>
    /// <returns></returns>
    private IEnumerator GenerateCorridorsNode(BSPTreeNode node, WaitForSeconds delayPerCorridor, WaitForSeconds delayPerTile)
    {
        if (!node.IsInternal)
        {
            yield break;
        }
        
        RectInt leftContainer = node.left.container;
        RectInt rightContainer = node.right.container;
            
        Vector2Int leftCenter = Vector2Int.RoundToInt(leftContainer.center);
        Vector2Int rightCenter = Vector2Int.RoundToInt(rightContainer.center);
        Vector2Int direction = (rightCenter - leftCenter);
        
        //'Normalize' the vector to either be right or up based on higher value
        direction = direction.x > direction.y ? Vector2Int.right : Vector2Int.up;


        while (Mathf.RoundToInt(Vector2.Distance(leftCenter, rightCenter)) > 0.5f)
        {
            if (direction.Equals(Vector2Int.right))
            {
                for (int i = 0; i < corridorThickness; i++)
                {
                    map.SpawnTile(new Vector2Int((int) leftCenter.x, (int) leftCenter.y + i), corridor);
                }

            }else if (direction.Equals(Vector2Int.up))
            {
                for (int i = 0; i < corridorThickness; i++)
                {
                    map.SpawnTile(new Vector2Int((int) leftCenter.x + i, (int) leftCenter.y), corridor);
                }
            }
                
            yield return delayPerTile;
            
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

        //solo tiles
        if (bmGridTile != null && mlGridTile == null && mrGridTile == null && tmGridTile == null) return tSoloTile;
        if (tmGridTile != null && mlGridTile == null && mrGridTile == null && bmGridTile == null) return bSoloTile;
        if (mrGridTile != null && tmGridTile == null && bmGridTile == null && mlGridTile == null) return lSoloTile;
        if (mlGridTile != null && tmGridTile == null && bmGridTile == null && mrGridTile == null) return rSoloTile;
		
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

    /// <summary>
    /// Paint tiles of the map to be correct 
    /// </summary>
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

    /// <summary>
    /// Draw the start and end point of the map to the texture
    /// </summary>
    public void DrawStartAndEndPointToTex()
    {
        tree.ChooseStartAndEndNode();
        
        Vector2Int start =
            new Vector2Int((int) tree.StartRoom.container.center.x, (int) tree.StartRoom.container.center.y);
        
        startMapTexture.SetPixel(start.x, start.y, Color.green);
        startMapTexture.ProtectPixel(start);
        
        Vector2Int destination =
            new Vector2Int((int) tree.EndRoom.container.center.x, (int) tree.EndRoom.container.center.y);
        
        startMapTexture.SetPixel(destination.x, destination.y, Color.red);
        startMapTexture.ProtectPixel(destination);
    }

    /// <summary>
    /// Generate a Dijkstra Map from the start of the level 
    /// </summary>
    /// <returns></returns>
    public IEnumerator GenerateDistFromStartMap(float delayTime)
    {
        distFromStartMap = new DijkstraMap();
        distFromStartMap.Initialize(new Vector2Int((int)tree.StartRoom.container.center.x, (int)tree.StartRoom.container.center.y)
            , map.TileMap);

        WaitForSeconds delay = new WaitForSeconds(delayTime);
        
        while (!distFromStartMap.IsFinished)
        {
            const int updatesPerFrame = 6;
            for (int i = 0; i < updatesPerFrame; ++i)
            {
                distFromStartMap.Update();
            }
            distFromStartMap.GetMapAsTexture(ref startMapTexture, distGradient);
            yield return delay;
        }
    }

    /// <summary>
    /// Generate and draw the hot path from the end to start of the level
    /// </summary>
    /// <param name="delayPerCube"></param>
    /// <returns></returns>
    public IEnumerator GenerateAndDrawHotPath(float delayPerCube)
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

    /// <summary>
    /// Mark rooms that are on the hot path
    /// </summary>
    /// <param name="route"></param>
    private void MarkHotPathRooms(List<Vector2Int> route)
    {
        List<BSPTreeNode> leafNodes = new List<BSPTreeNode>();
        tree.GetLeafNodes(ref leafNodes);
        foreach (BSPTreeNode node in leafNodes)
        {
            node.DetermineIsOnHotPath(route);
        }
    }

    /// <summary>
    /// Colour rooms that are on the hot path
    /// </summary>
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
                            startMapTexture.SetPixel(mainMapPos.x, mainMapPos.y, Color.blue);
                        }
                        
                    }
                }
            }
        }
        startMapTexture.Apply();
    }
}
