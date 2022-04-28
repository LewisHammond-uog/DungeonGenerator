using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private BSPTree tree;

    [SerializeField] private int numberOfItterations = 10;

    [SerializeField] private Vector2Int dungeonSize;
    [SerializeField] private Vector2Int minRoomSize;

    [SerializeField] private RoomInfo[] rooms;

    [SerializeField] private GameObject corridor;

    [SerializeField] private List<GameObject> spawnedRooms;
    
    [HideInInspector]
    public GameObject tlTile;
    [HideInInspector]
    public GameObject tmTile;
    [HideInInspector]
    public GameObject trTile;
    [HideInInspector]
    public GameObject mlTile;
    [HideInInspector]
    public GameObject mmTile;
    [HideInInspector]
    public GameObject mrTile;
    [HideInInspector]
    public GameObject blTile;
    [HideInInspector]
    public GameObject bmTile;
    [HideInInspector]
    public GameObject brTile;

    private TileMap3D map;
    
    
    // Start is called before the first frame update
    void Start()
    {
        map = gameObject.AddComponent<TileMap3D>();
        map.Init(dungeonSize);
        
        
        GenerateSpace();
        GenerateCorridors();
        //StartCoroutine(GetComponent<BSPGraphVisualizer>().DrawTree(tree.RootNode, Vector2.zero));
        //PaintTilesAccordingToTheirNeighbors();
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
            StartCoroutine(GenerateCorridorsNode(tree.RootNode));
        }
    }
    
    public IEnumerator GenerateCorridorsNode(BSPTreeNode node)
    {
        if (node.IsInternal)
        {
            RectInt leftContainer = node.left.container;
            RectInt rightContainer = node.right.container;
            
            
            Vector2 leftCenter = leftContainer.center;
            Vector2 rightCenter = rightContainer.center;
            Vector2 direction = (rightCenter - leftCenter).normalized;
            
            while (Vector2.Distance(leftCenter, rightCenter) > 1)
            {
                if (direction.Equals(Vector2.right))
                {
                    map.SpawnTile(Vector2Int.RoundToInt(leftCenter), corridor);

                }else if (direction.Equals(Vector2.up))
                {
                    map.SpawnTile(Vector2Int.RoundToInt(leftCenter), corridor);
                }
                
                leftCenter.x += direction.x;
                leftCenter.y += direction.y;
            }
            
            if (node.left != null)
            {
                yield return GenerateCorridorsNode(node.left);
            }

            if (node.right != null)
            {
                yield return GenerateCorridorsNode(node.right);
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
