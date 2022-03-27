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

    public Vector2Int minSize = new Vector2Int(250, 250);
    
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

        const float minSplitSize = 0.25f;
        const float maxSplitSize = 0.5f;
        
        if (direction == SplitDirection.Vertical)
        {
            c1 = new RectInt (container.x, container.y,
                container.width, (int) UnityEngine.Random.Range(container.height * minSplitSize, container.height * maxSplitSize));
            c2 = new RectInt (container.x, container.y + c1.height,
                container.width, container.height - c1.height);
        }else
        {
            c1 = new RectInt (container.x, container.y,
                (int) UnityEngine.Random.Range (container.width * minSplitSize, container.width * maxSplitSize), container.height);
            c2 = new RectInt (container.x + c1.width, container.y,
                container.width - c1.width, container.height);
        }

        return new RectInt[] {c1, c2};
    }

    private bool IsContainerGreaterThanMinSize(RectInt container)
    {
        return container.size.x > minSize.x && container.size.y > minSize.y;
    }
}

public enum SplitDirection
{
    Vertical,
    Horizontal,

    DirectionCount
}