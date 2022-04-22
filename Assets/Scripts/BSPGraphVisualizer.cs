using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BSPGraphVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject canvasParent;
    [SerializeField] private GameObject layerPrefab;
    
    public void Start()
    {
        
    }

    public void DrawTree(BSPTree tree)
    {
        Dictionary<int, List<BSPTreeNode>> nodesPerLevel = LevelBasedSearch(tree);

        int currentLevel = 0;
        int nodeIndex = 0;
        while (nodesPerLevel.ContainsKey(currentLevel))
        {
            //Create a new row
            GameObject row = Instantiate(layerPrefab,canvasParent.transform);
            RowVisualizer rowVis = row.GetComponent<RowVisualizer>();
            
            List<BSPTreeNode> nodesInLevel = nodesPerLevel[currentLevel];
            foreach (BSPTreeNode node in nodesInLevel)
            {
                string nodeLetter = GetNodeLetter(nodeIndex);
                
                //Add item to row
                rowVis.AddItem(nodeLetter);
                
                nodeIndex++;
            }

            currentLevel++;
        }

    }
    

    public Dictionary<int, List<BSPTreeNode>> LevelBasedSearch(BSPTree tree)
    {
        Dictionary<int, List<BSPTreeNode>> nodesPerLevel = new Dictionary<int, List<BSPTreeNode>>();   

        int height = tree.GetHeight();

        for (int i = 0; i < height; ++i)
        {
            List<BSPTreeNode> nodes = new List<BSPTreeNode>();
            GetNodesOfCurrentLevel(tree.RootNode, i, ref nodes);
            nodesPerLevel.Add(i, nodes);
        }

        return nodesPerLevel;
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
    
    /// <summary>
    /// Get the letter of the node from a given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string GetNodeLetter(int index)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string value = "";

        if (index >= letters.Length)
        {
            value += letters[index / letters.Length - 1];
        }

        value += letters[index % letters.Length];

        return value;
    }
}
