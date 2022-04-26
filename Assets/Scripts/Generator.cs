using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private BSPTree tree;

    [SerializeField] private int numberOfItterations = 10;

    [SerializeField] private Vector2Int dungeonSize;
    [SerializeField] private Vector2Int minRoomSize;

    [SerializeField] private GameObject roomPrefab;

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
            GameObject obj = Instantiate(roomPrefab);
            obj.transform.position = node.container.center;
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
                    Instantiate(corridor, new Vector3(leftCenter.x, leftCenter.y), Quaternion.Euler(90, 0, 0));

                }else if (direction.Equals(Vector2.up))
                {
                    Instantiate(corridor, new Vector3(leftCenter.x, leftCenter.y), Quaternion.Euler(90, 0, 0));
                }

                yield return new WaitForSeconds(0.25f);
                
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
