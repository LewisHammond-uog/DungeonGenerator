using System;
using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private Generator generator;

    [Header("Space")] 
    [SerializeField] private float spaceDrawDelay = 0.5f;
    
    [Header("Corridors")]
    [SerializeField] private float delayPerCorridorTile = 0.01f;
    [SerializeField] private float delayPerCorridor = 0.1f;

    [Header("Hot Path")] 
    [SerializeField] private float startMapDelay = 0.001f;
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

        yield return DrawSpaceAndTreeProgressive(generator.tree.RootNode);
        yield return new WaitForSeconds(delay);
        
        generator.GenerateRooms();
        yield return new WaitForSeconds(delay);
        
        yield return generator.GenerateCorridors(delayPerCorridor, delayPerCorridorTile);
        yield return new WaitForSeconds(delay);
        
        generator.PaintTiles();
        yield return new WaitForSeconds(delay);

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
