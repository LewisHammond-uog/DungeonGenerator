using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BSPGraphVisualizer : MonoBehaviour
{
    [SerializeField] private Transform canvasParent;
    [SerializeField] private GameObject itemPrefab;

    private int index;

    /// <summary>
    /// Draw a tree
    /// </summary>
    /// <param name="treeRoot"></param>
    /// <param name="pos"></param>
    public IEnumerator DrawTree(BSPTreeNode treeRoot, Vector2 pos)
    {
        yield return null;
        
        //Draw the head of the tree
        DrawNode(treeRoot, pos);
        treeRoot.isDrawn = true;
        

        const float xSpacing = 100f;
        const float ySpacing = 150f;
        
        if (treeRoot.left != null)
        {
            yield return DrawTree(treeRoot.left, new Vector2(pos.x + xSpacing, pos.y - ySpacing));
        }
                
        if (treeRoot.right != null)
        {
            yield return DrawTree(treeRoot.right, new Vector2(pos.x - xSpacing, pos.y - ySpacing));
        }
    }

    /// <summary>
    /// Draw a node in the tree
    /// </summary>
    /// <param name="node"></param>
    /// <param name="pos"></param>
    private void DrawNode(BSPTreeNode node, Vector2 pos)
    {
        //Spawn
        Transform parentTransform = canvasParent.transform;
        GameObject obj = Instantiate(itemPrefab, canvasParent.transform);
        
        //Move to pos
        Vector3 parentPos = parentTransform.position;
        obj.transform.position = new Vector3(parentPos.x + pos.x, parentPos.y + pos.y);
        
        //Set letter
        obj.GetComponentInChildren<TMP_Text>().text = GetNodeLetter(index);
        index++;
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
