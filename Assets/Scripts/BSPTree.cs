using UnityEngine;

public class BSPTree
{
    public BSPTreeNode rootNode { get; private set; }
    
    public BSPTree(int numberOfItterations, Vector2Int dungeonSize, Vector2Int minRoomSize)
    {
        BSPTreeNode.MinRoomSize = minRoomSize;

        RectInt dungeonBounds = new RectInt(new Vector2Int(0, 0), dungeonSize);
        
        rootNode = new BSPTreeNode(dungeonBounds);
        rootNode = rootNode.Split(numberOfItterations, dungeonBounds);
    }
}