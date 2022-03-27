using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BSPTreeNode
{
    public RectInt container;
    public BSPTreeNode parent;
    public BSPTreeNode left;
    public BSPTreeNode right;

    //The minium size to allow a split
    private static Vector2Int MinSplitSize;
    public static Vector2Int MinRoomSize
    {
        set
        {
            MinSplitSize = new Vector2Int((int)(value.x / MinSplitRatio), (int)(value.y / MinSplitRatio));
        }
    }

    private const float MinSplitRatio = 0.25f;
    private const float MaxSplitRatio = 0.5f;

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
            treeNode.left = Split(numberOfIterations - 1, splitContainers[0]);
            treeNode.left.parent = this;
        }

        if (IsContainerGreaterThanMinSize(splitContainers[1]))
        {
            treeNode.right = Split(numberOfIterations - 1, splitContainers[1]);
            treeNode.right.parent = this;
        }
        
        return treeNode;
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
        return container.size.x > MinSplitSize.x && container.size.y > MinSplitSize.y;
    }
}

public enum SplitDirection
{
    Vertical,
    Horizontal,

    DirectionCount
}