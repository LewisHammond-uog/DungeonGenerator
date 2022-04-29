using System.Collections.Generic;
using UnityEngine;

public class BSPTree
{
    public BSPTreeNode RootNode { get; private set; }

    public BSPTreeNode StartRoom { get; private set; }
    public BSPTreeNode EndRoom { get; private set; }

    public BSPTree(int numberOfItterations, Vector2Int dungeonSize, Vector2Int minRoomSize)
    {
        BSPTreeNode.MinRoomSize = minRoomSize;

        RectInt dungeonBounds = new RectInt(new Vector2Int(0, 0), dungeonSize);
        
        RootNode = new BSPTreeNode(dungeonBounds);
        RootNode = RootNode.Split(numberOfItterations, dungeonBounds);
    }

    /// <summary>
    /// Get the height of the entire tree
    /// </summary>
    /// <returns></returns>
    public int GetHeight()
    {
        return GetHeight(RootNode);
    }

    /// <summary>
    /// Get the height from a given root node
    /// </summary>
    /// <param name="root">Root to start from</param>
    /// <returns></returns>
    public static int GetHeight(BSPTreeNode root)
    {
        if (root == null)
        {
            return 0;
        }
        
        //Compute the height of each sub-tree
        int lHeight = GetHeight(root.left);
        int rHeight = GetHeight(root.right);

        //Return highest, +1 so it is not zero indexed
        return Mathf.Max(lHeight, rHeight) + 1;
    }

    public void GetLeafNodes(ref List<BSPTreeNode> leafNodes)
    {
        GetLeafNodes(RootNode, ref leafNodes);
    }
    
    public static void GetLeafNodes(BSPTreeNode root, ref List<BSPTreeNode> leafNodes)
    {
        if (root == null)
        {
            return;
        }
        
        //If both nodes are null then this is a leaf
        if (root.left == null && root.right == null)
        {
            leafNodes.Add(root);
        }
        
        //If left or right exists, recurse these nodes
        if (root.left != null)
        {
            GetLeafNodes(root.left, ref leafNodes);
        }

        if (root.right != null)
        {
            GetLeafNodes(root.right, ref leafNodes);
        }
    }

    /// <summary>
    /// Choose a start and end node
    /// </summary>
    public void ChooseStartAndEndNode()
    {
        List<BSPTreeNode> leafNodes = new List<BSPTreeNode>();
        GetLeafNodes(ref leafNodes);

        if (leafNodes.Count < 2)
        {
            Debug.LogError("Not enough nodes for start ned");
            return;
        }

        int startIndex = Random.Range(0, leafNodes.Count);
        int endIndex = Random.Range(0, leafNodes.Count);
        //Ensure end node is not start node
        while (endIndex == startIndex)
        {
            endIndex = Random.Range(0, leafNodes.Count);
        }

        StartRoom = leafNodes[startIndex];
        EndRoom = leafNodes[endIndex];
    }
    
}