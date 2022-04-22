using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RowVisualizer : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject itemPrefab;

    public void AddItem(string letter)
    {
        GameObject item = Instantiate(itemPrefab, parent);
        //Get text and set letter
        TMP_Text textComp = item.GetComponentInChildren<TMP_Text>();
        if (textComp)
        {
            textComp.text = letter;
        }
    }

}
