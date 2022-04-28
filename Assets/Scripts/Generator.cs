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
        StartCoroutine(GetComponent<BSPGraphVisualizer>().DrawTree(tree.RootNode, Vector2.zero));
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
            GameObject obj = Instantiate(RoomUtils.GetRoomPrefabForNode(node, rooms));
            obj.transform.position = new Vector3((int)node.container.center.x, (int)node.container.center.y);
            spawnedRooms.Add(obj);
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
                    map.SetTile(Vector2Int.RoundToInt(leftCenter), corridor);
                    //GameObject spawnedCorridor = Instantiate(corridor, new Vector3((int)leftCenter.x, (int)leftCenter.y), Quaternion.Euler(90, 0, 0));

                }else if (direction.Equals(Vector2.up))
                {
                    map.SetTile(Vector2Int.RoundToInt(leftCenter), corridor);
                    GameObject spawnedCorridor = Instantiate(corridor, new Vector3((int)leftCenter.x, (int)leftCenter.y), Quaternion.Euler(90, 0, 0));
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

    public GameObject GetTile(Vector3Int pos)
    {
        Vector3 halfExtends =
            new Vector3(corridor.transform.localScale.x / 2, corridor.transform.localScale.y / 2, 10f);

        Vector3 posV3 = new Vector3(pos.x, pos.y, 0);
        
        Collider[] hits = Physics.OverlapBox(posV3, halfExtends);

        if (hits.Length == 0)
        {
            return null;
        }
        else
        {
            return hits[0].gameObject;
        }
    }
    
    private void PaintTilesAccordingToTheirNeighbors () {
        for (int i = 0; i < dungeonSize.x; i++) {
            for (int j = 0; j < dungeonSize.y; j++) {
                var tile = GetTileByNeighbours (i, j);
                if (tile != null)
                {
                    Instantiate(tile, new Vector3(i, j), Quaternion.identity);
                }
            }
        }
    }

    public GameObject GetTileByNeighbours(int i, int j)
    {
        GameObject mmGridTile = GetTile (new Vector3Int (i, j, 0));
        if (mmGridTile == null) return null; // you shouldn't repaint a n

        GameObject blGridTile = GetTile (new Vector3Int (i-1, j-1, 0));
        GameObject bmGridTile = GetTile (new Vector3Int (i,   j-1, 0));
        GameObject brGridTile = GetTile (new Vector3Int (i+1, j-1, 0));

        GameObject mlGridTile = GetTile (new Vector3Int (i-1, j, 0));
        GameObject mrGridTile = GetTile (new Vector3Int (i+1, j, 0));

        GameObject tlGridTile = GetTile (new Vector3Int (i-1, j+1, 0));
        GameObject tmGridTile = GetTile (new Vector3Int (i,   j+1, 0));
        GameObject trGridTile = GetTile (new Vector3Int (i+1, j+1, 0));

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
    
    
#if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        //GenerateCorridors();
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
        Gizmos.DrawLine (new Vector3 (node.container.x, node.container.y, 0), new Vector3Int (node.container.xMax, node.container.y, 0));
        // right
        Gizmos.DrawLine (new Vector3 (node.container.xMax, node.container.y, 0), new Vector3Int (node.container.xMax, node.container.yMax, 0));
        // bottom
        Gizmos.DrawLine (new Vector3 (node.container.x, node.container.yMax, 0), new Vector3Int (node.container.xMax, node.container.yMax, 0));
        // left
        Gizmos.DrawLine (new Vector3 (node.container.x, node.container.y, 0), new Vector3Int (node.container.x, node.container.yMax, 0));

        // children
        if (node.left != null) DebugDrawBspNode (node.left);
        if (node.right != null) DebugDrawBspNode (node.right);
    }
#endif
    
}
