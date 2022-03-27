using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private BSPTreeNode treeNode;

    [SerializeField] private int numberOfItterations = 10;

    [SerializeField] private Vector2 minRoomSize;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateSpace();
    }
    private void GenerateSpace()
    {
        RectInt rect = new RectInt(0, 0, 1000, 1000);
        treeNode = new BSPTreeNode(rect);
        treeNode = treeNode.Split(numberOfItterations, rect);
    }
    
    
    void OnDrawGizmos ()
    {
        DebugDrawBsp();
    }

    public void DebugDrawBsp () {
        if (treeNode == null) return; // hasn't been generated yet

        DebugDrawBspNode (treeNode); // recursive call
    }

    public void DebugDrawBspNode (BSPTreeNode node) {
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
    
}
