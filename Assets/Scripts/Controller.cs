using System;
using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private Generator generator;

    [Header("Corridors")]
    [SerializeField] private float delayPerCorridorTile = 0.01f;
    [SerializeField] private float delayPerCorridor = 0.1f;

    [Header("Hot Path")] 
    [SerializeField] private float hotPathDelay = 0.05f;
    
    
    private void Awake()
    {
        StartCoroutine(DoSequence());
    }

    private IEnumerator DoSequence()
    {
        const float delay = 1f;
        
        generator.Init();

        generator.GenerateSpace();
        yield return new WaitForSeconds(delay);
        
        yield return generator.GenerateCorridors(delayPerCorridor, delayPerCorridorTile);
        yield return new WaitForSeconds(delay);
        
        generator.PaintTiles();
        yield return new WaitForSeconds(delay);

        yield return generator.GenerateDistFromStartMap();

        yield return generator.GenerateAndDrawHotPath(hotPathDelay);
    }
}
