using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPTree
{
    public RectInt container;
    public RectInt room;
    public BSPTree left;
    public BSPTree right;

    public BSPTree(RectInt container)
    {
        this.container = container;
    }

    public BSPTree Split(int numberOfIterations, RectInt container)
    {
        BSPTree treeNode = new BSPTree(container);
        if (numberOfIterations <= 0)
        {
            return treeNode;
        }

        RectInt[] splitContainers = SplitContainer(container);
        treeNode.left = Split (numberOfIterations - 1, splitContainers[0]);
        treeNode.right = Split (numberOfIterations - 1, splitContainers[1]);

        return treeNode;
    }

    private RectInt[] SplitContainer(RectInt container)
    {
        RectInt c1, c2;
        
        //Decide if we should split vertically or horizontally
        int randValue = Random.Range(0, (int)SplitDirection.DirectionCount);
        Debug.Log(randValue);
        SplitDirection direction = (SplitDirection) randValue;

        if (direction == SplitDirection.Vertical)
        {
            c1 = new RectInt (container.x, container.y,
                container.width, (int) UnityEngine.Random.Range(container.height * 0.3f, container.height * 0.5f));
            c2 = new RectInt (container.x, container.y + c1.height,
                container.width, container.height - c1.height);
        }else
        {
            c1 = new RectInt (container.x, container.y,
                (int) UnityEngine.Random.Range (container.width * 0.3f, container.width * 0.5f), container.height);
            c2 = new RectInt (container.x + c1.width, container.y,
                container.width - c1.width, container.height);
        }

        return new RectInt[] {c1, c2};
    }
}

public enum SplitDirection
{
    Vertical,
    Horizontal,

    DirectionCount
}