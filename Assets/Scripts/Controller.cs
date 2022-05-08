using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField] private Generator generator;

    [Header("Space")] 
    [SerializeField] private float spaceDrawDelay = 0.5f;
    
    [Header("Corridors")]
    [SerializeField] private float delayPerCorridorTile = 0.01f;
    [SerializeField] private float delayPerCorridor = 0.1f;

    [Header("DLA")] 
    [SerializeField] private ParticleSpawner dlaSpawner;
    
    [Header("Hot Path")] 
    [SerializeField] private GameObject hotPathTex;
    [SerializeField] private float startMapDelay = 0.001f;
    [SerializeField] private float hotPathDelay = 0.05f;

    [Header("UI")] 
    [SerializeField] private Slider boundsX;
    [SerializeField] private Slider boundsY;
    [SerializeField] private Slider minCellX;
    [SerializeField] private Slider minCellY;
    [SerializeField] private Slider bspItterations;
    [SerializeField] private Toggle enableDLA;
    [SerializeField] private Slider dlaParticleCount;

    public void StartSequence()
    {
        StartCoroutine(DoSequence());
    }

    private IEnumerator DoSequence()
    {
        const float delay = 1f;
        
        Vector2Int roomSize = new Vector2Int((int)boundsX.value, (int)boundsY.value);
        Vector2Int cellSize = new Vector2Int((int)minCellX.value, (int)minCellY.value);
        
        generator.Init(roomSize, cellSize, (int)bspItterations.value);
        
        generator.GenerateSpace();
        yield return new WaitForSeconds(delay);

        yield return DrawSpaceAndTreeProgressive(generator.tree.RootNode);
        yield return new WaitForSeconds(delay);
        
        generator.GenerateRooms();
        yield return new WaitForSeconds(delay);
        
        yield return generator.GenerateCorridors(delayPerCorridor, delayPerCorridorTile);
        yield return new WaitForSeconds(delay);
        generator.PaintTiles();
        
        //Erode Rooms with DLA
        if (enableDLA.isOn)
        {
            dlaSpawner.SpawnParticles((int)dlaParticleCount.value);
            yield return dlaSpawner.WaitForAllParticlesDead();
            generator.PaintTiles();
        }

        yield return new WaitForSeconds(delay);

        hotPathTex.SetActive(true);
        yield return generator.GenerateDistFromStartMap(startMapDelay);

        yield return generator.GenerateAndDrawHotPath(hotPathDelay);
    }

    private IEnumerator DrawSpaceAndTreeProgressive(BSPTreeNode node )
    {
        //Draw Nodes
        DrawRectangle(node);

        yield return new WaitForSeconds(spaceDrawDelay);
        
        if (node.left != null) yield return DrawSpaceAndTreeProgressive (node.left);
        if (node.right != null) yield return DrawSpaceAndTreeProgressive (node.right);
    }

    private void DrawRectangle(BSPTreeNode node)
    {
        GameObject rectangle = new GameObject("Rectangle");
        LineRenderer lineRender = rectangle.AddComponent<LineRenderer>();

        lineRender.positionCount = 4;
        lineRender.loop = true;
        
        //Draw lines
        lineRender.SetPosition(0, new Vector3 (node.container.xMax, 0, node.container.yMax));
        lineRender.SetPosition(1, new Vector3 (node.container.xMax, 0, node.container.yMin));
        lineRender.SetPosition(2, new Vector3 (node.container.xMin, 0, node.container.yMin));
        lineRender.SetPosition(3, new Vector3 (node.container.xMin, 0, node.container.yMax));
    }
}
