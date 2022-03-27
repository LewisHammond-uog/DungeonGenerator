using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private BSPTree tree;

    [SerializeField] private int numberOfItterations = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateSpace();
    }
    private void GenerateSpace()
    {
        RectInt rect = new RectInt(0, 0, 1000, 1000);
        tree = new BSPTree(rect);
        tree = tree.Split(numberOfItterations, rect);
    }
    
    
    void OnDrawGizmos ()
    {
        DebugDrawBsp();
    }

    public void DebugDrawBsp () {
        if (tree == null) return; // hasn't been generated yet

        DebugDrawBspNode (tree); // recursive call
    }

    public void DebugDrawBspNode (BSPTree node) {
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
