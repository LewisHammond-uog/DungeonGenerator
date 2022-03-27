using UnityEngine;

public class BSPTree
{
    public BSPTreeNode RootNode { get; private set; }
    
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
}