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
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateSpace();
        GenerateCorridors();
        StartCoroutine(GetComponent<BSPGraphVisualizer>().DrawTree(tree.RootNode, Vector2.zero));
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
                    GameObject spawnedCorridor = Instantiate(corridor, new Vector3((int)leftCenter.x, (int)leftCenter.y), Quaternion.Euler(90, 0, 0));
                    RemoveWallsAtPos(spawnedCorridor);

                }else if (direction.Equals(Vector2.up))
                {
                    GameObject spawnedCorridor = Instantiate(corridor, new Vector3((int)leftCenter.x, (int)leftCenter.y), Quaternion.Euler(90, 0, 0));
                    RemoveWallsAtPos(spawnedCorridor);
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

    private void RemoveWallsAtPos(GameObject corridor)
    {
        Vector3 posV3 = new Vector3(corridor.transform.position.x, corridor.transform.position.y, 0);
        
        //Take the size of the corrodior blocks but add some height to fix issues with no overlap with some rooms
        Vector3 halfExtends =
            new Vector3(corridor.transform.localScale.x / 2, corridor.transform.localScale.y / 2, 10f);
        
        Collider[] hits = Physics.OverlapBox(posV3, halfExtends);

        List<GameObject> delObjs = new List<GameObject>();
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.CompareTag("Wall"))
            {
                delObjs.Add(hit.gameObject);
                Debug.Log("Wall");
            }
        }

        foreach (GameObject delObj in delObjs)
        {
            Destroy(delObj);
            Debug.Log("Des");
        }
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
