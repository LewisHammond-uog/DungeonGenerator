using System;
using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private Generator generator;

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
        
        generator.GenerateCorridors();
        yield return new WaitForSeconds(delay);
        
        generator.PaintTiles();
        yield return new WaitForSeconds(delay);

        yield return generator.GenerateDistFromStartMap();

        yield return generator.DrawHotPath();
    }
}
