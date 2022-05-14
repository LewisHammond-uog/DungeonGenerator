using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for storing the description and title for a stage in the process
[CreateAssetMenu(menuName = "Create Description")]
public class Description : ScriptableObject
{
    [SerializeField] public string title;
    [SerializeField] [TextArea(10,20)] public string description;
}
