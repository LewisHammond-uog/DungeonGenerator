using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BSPTreeNode
{
    public RectInt container;
    public RectInt room;
    public BSPTreeNode parent;
    public BSPTreeNode left;
    public BSPTreeNode right;


    public bool IsOnHotPath { private set; get; } = false;

    //The minium size to allow a split
    private static Vector2Int minSplitSize;
    public static Vector2Int MinRoomSize
    {
        set => minSplitSize = new Vector2Int((int)(value.x / MinSplitRatio), (int)(value.y / MinSplitRatio));
    }

    private const float MinSplitRatio = 0.3f;
    private const float MaxSplitRatio = 0.5f;

    public bool IsLeaf => left == null && right == null;
    public bool IsInternal => left != null || right != null;
    
    
    public BSPTreeNode(RectInt container)
    {
        this.container = container;
    }

    public BSPTreeNode Split(int numberOfIterations, RectInt container)
    {
        BSPTreeNode treeNode = new BSPTreeNode(container);
        if (numberOfIterations <= 0)
        {
            return treeNode;
        }

        RectInt[] splitContainers = SplitContainer(container);

        if (IsContainerGreaterThanMinSize(splitContainers[0]))
        {
            treeNode.left = treeNode.Split(numberOfIterations - 1, splitContainers[0]);
            //set the parent to THIS, means that the tree node we created must call the split method
            treeNode.left.parent = this;
        }
        else
        {
            treeNode.left = new BSPTreeNode(splitContainers[0]);
            treeNode.left.parent = this;
        }

        if (IsContainerGreaterThanMinSize(splitContainers[1]))
        {
            treeNode.right = treeNode.Split(numberOfIterations - 1, splitContainers[1]);
            treeNode.right.parent = this;
        }
        else
        {
            treeNode.right = new BSPTreeNode(splitContainers[1]);
            treeNode.right.parent = this;
        }
        
        return treeNode;
    }

    /// <summary>
    /// Determine if this node is on the hotpath
    /// </summary>
    public void DetermineIsOnHotPath(List<Vector2Int> route)
    {
        foreach (Vector2Int point in route)
        {
            if (!container.Contains(point))
            {
                continue;
            }
            
            IsOnHotPath = true;
            return;
        }
    }

    /// <summary>
    /// Split a room container in to 2 containers
    /// </summary>
    /// <param name="container">Container to split</param>
    /// <returns></returns>
    private RectInt[] SplitContainer(RectInt container)
    {
        RectInt c1, c2;
        
        //Decide if we should split vertically or horizontally
        SplitDirection direction = SplitDirection.Vertical;
        float directionRandVal = Random.value;
        if (directionRandVal > 0.5f)
        {
            direction = SplitDirection.Horizontal;
        }

        if (direction == SplitDirection.Vertical)
        {
            c1 = new RectInt (container.x, container.y,
                container.width, (int) UnityEngine.Random.Range(container.height * MinSplitRatio, container.height * MaxSplitRatio));
            c2 = new RectInt (container.x, container.y + c1.height,
                container.width, container.height - c1.height);
        }else
        {
            c1 = new RectInt (container.x, container.y,
                (int) UnityEngine.Random.Range (container.width * MinSplitRatio, container.width * MaxSplitRatio), container.height);
            c2 = new RectInt (container.x + c1.width, container.y,
                container.width - c1.width, container.height);
        }

        return new RectInt[] {c1, c2};
    }

    /// <summary>
    /// Is this container less than the minimum allowed size
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    private bool IsContainerGreaterThanMinSize(RectInt container)
    {
        return container.size.x > minSplitSize.x && container.size.y > minSplitSize.y;
    }
}

public enum SplitDirection
{
    Vertical,
    Horizontal,

    DirectionCount
}