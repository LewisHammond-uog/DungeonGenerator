using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private BSPTree tree;

    [SerializeField] private int numberOfItterations = 10;

    [SerializeField] private Vector2Int dungeonSize;
    [SerializeField] private Vector2Int minRoomSize;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateSpace();
    }
    private void GenerateSpace()
    {
        tree = new BSPTree(numberOfItterations, dungeonSize, minRoomSize);
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        DebugDrawBsp();
    }

    private void DebugDrawBsp () {
        if (tree == null) return; // hasn't been generated yet
        if (tree.rootNode == null) return; // hasn't been generated yet

        DebugDrawBspNode (tree.rootNode); // recursive call
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
