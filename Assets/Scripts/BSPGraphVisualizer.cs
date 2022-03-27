using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BSPGraphVisualizer : MonoBehaviour
{
    public void LevelBasedSearch(BSPTree tree)
    {
        int height = tree.GetHeight();

        for (int i = 0; i < height; ++i)
        {
            List<BSPTreeNode> nodes = new List<BSPTreeNode>();
            GetNodesOfCurrentLevel(tree.RootNode, i, ref nodes);
            Debug.Log(nodes.Count);
        }

    }
    
    
    public void GetNodesOfCurrentLevel(BSPTreeNode root, int level, ref List<BSPTreeNode> levelList)
    {
        if (root == null)
        {
            return;
        }
        
        if (level == 1)
        {
            levelList.Add(root);
        }else if (level > 1)
        {
            GetNodesOfCurrentLevel(root.left, level -1, ref levelList);
            GetNodesOfCurrentLevel(root.right, level -1, ref levelList);
        }
    }
}
