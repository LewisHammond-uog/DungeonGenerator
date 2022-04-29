using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    private BSPTree tree;

    [SerializeField] private int numberOfItterations = 10;
    [SerializeField] private int corridorThickness = 2;

    [SerializeField] private Vector2Int dungeonSize;
    [SerializeField] private Vector2Int minRoomSize;
    [SerializeField] private GameObject debugTile;

    [SerializeField] private RoomInfo[] rooms;

    [SerializeField] private GameObject corridor;

    [SerializeField] private List<GameObject> spawnedRooms;
    
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
    private DijkstraMap distFromStartMap;

    [SerializeField] private Image testImage;
    [SerializeField] private Gradient distGradient;
    private Texture2D startMapTexture;

    // Start is called before the first frame update
    void Start()
    {
        map = gameObject.AddComponent<TileMap3D>();
        map.Init(dungeonSize);
        
        
        GenerateSpace();
        GenerateCorridors();
        PaintTiles();
        tree.ChooseStartAndEndNode();
        //StartCoroutine(GetComponent<BSPGraphVisualizer>().DrawTree(tree.RootNode, Vector2.zero));

        //Sprite for dist from start map
        startMapTexture = new Texture2D(dungeonSize.x, dungeonSize.y);
        Sprite distanceMapSprite = Sprite.Create(startMapTexture, new Rect(0,0,dungeonSize.x,dungeonSize.y), new Vector2(0,0));
        testImage.sprite = distanceMapSprite;
        testImage.rectTransform.sizeDelta = dungeonSize;
        testImage.transform.parent.position = new Vector3(dungeonSize.x / 2 - 0.5f, 1, dungeonSize.y / 2 - 4 + 0.5f);
        
        
        distFromStartMap = new DijkstraMap();
        distFromStartMap.Initialize(new Vector2Int((int)tree.StartRoom.container.center.x, (int)tree.StartRoom.container.center.y)
            , map.TileMap);
    }

    private void Update()
    {
        distFromStartMap.Update();
        distFromStartMap.GetMapAsTexture(ref startMapTexture, distGradient);
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
            map.InsertRoom(spawnPos, RoomUtils.GetRoomPrefabForNode(node, rooms));
        }
        
    }

    private void GenerateCorridors()
    {
        if (tree != null && tree.RootNode != null)
        {
           GenerateCorridorsNode(tree.RootNode);
        }
    }
    
    public void GenerateCorridorsNode(BSPTreeNode node)
    {
        if (node.IsInternal)
        {
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

                }else if (direction.Equals(Vector2.up))
                {
                    for (int i = 0; i < corridorThickness; i++)
                    {
                        map.SpawnTile(new Vector2Int((int) leftCenter.x + i, (int) leftCenter.y), corridor);
                    }
                }
                
                leftCenter.x += direction.x;
                leftCenter.y += direction.y;
            }
            
            if (node.left != null)
            {
                GenerateCorridorsNode(node.left);
            }

            if (node.right != null)
            {
               GenerateCorridorsNode(node.right);
            }
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

    private void PaintTiles()
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
