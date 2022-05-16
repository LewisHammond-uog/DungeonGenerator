using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Slider corridorThickness;
    [SerializeField] private Slider bspItterations;
    [SerializeField] private Toggle enableDLA;
    [SerializeField] private Slider dlaParticleCount;
    
    [SerializeField] private Button generateButton;
    [SerializeField] private Button resetButton;

    [Header("Text Display")] 
    [SerializeField] private TMP_Text titleField;
    [SerializeField] private TMP_Text descriptionField;
    [SerializeField] private Description[] descriptions;
    
    [Header("Camera")]
    [SerializeField] private Transform camDefaultPos;
    [SerializeField] private Transform camRotatePos;

    [Header("Advance Button")] 
    [SerializeField] private Button advanceButton;

    [SerializeField] private Toggle autoAdvance;
    
    //BSP
    private List<LineRenderer> drawnRectangles;

    private void Awake()
    {
        drawnRectangles = new List<LineRenderer>();
        ToggleInputUI(true);
        UpdateDescriptionSafe(0);
    }

    public void StartSequence()
    {
        StartCoroutine(DoSequence());
    }

    private IEnumerator DoSequence()
    {
        ToggleInputUI(false);
        Vector2Int roomSize = new Vector2Int((int)boundsX.value, (int)boundsY.value);
        Vector2Int cellSize = new Vector2Int((int)minCellX.value, (int)minCellY.value);
        
        generator.Init(roomSize, cellSize, (int)corridorThickness.value, (int)bspItterations.value);

        UpdateDescriptionSafe(1);
        generator.GenerateSpace();
        
        yield return DrawSpaceAndTreeProgressive(generator.tree.RootNode);
        
        generator.GenerateRooms();
        yield return WaitForAdvanceButtonPressIfNoAutoAdvance();
        
        UpdateDescriptionSafe(2);
        yield return generator.GenerateCorridors(delayPerCorridor, delayPerCorridorTile);
        generator.PaintTiles();
        yield return WaitForAdvanceButtonPressIfNoAutoAdvance();
        
        //Erode Rooms with DLA
        if (enableDLA.isOn)
        {
            UpdateDescriptionSafe(3);
            dlaSpawner.SpawnParticles((int)dlaParticleCount.value);
            yield return dlaSpawner.WaitForAllParticlesDead();
            generator.PaintTiles();
            yield return WaitForAdvanceButtonPressIfNoAutoAdvance();
        }
        

        hotPathTex.SetActive(true);
        UpdateDescriptionSafe(4);

        generator.DrawStartAndEndPointToTex();
        yield return generator.GenerateDistFromStartMap(startMapDelay);

        yield return WaitForAdvanceButtonPressIfNoAutoAdvance();
        
        UpdateDescriptionSafe(5);
        yield return generator.GenerateAndDrawHotPath(hotPathDelay);

        StartCoroutine(RotateCamForever());
        UpdateDescriptionSafe(6);
    }

    public void ResetGeneration()
    {
        //Reset rectangles
        foreach (LineRenderer rectangle in drawnRectangles)
        {
            Destroy(rectangle.gameObject);
        }
        
        drawnRectangles.Clear();
        generator.ResetGenerator();
        generator.StopAllCoroutines();
        StopAllCoroutines();
        
        dlaSpawner.ResetSpawner();
        
        hotPathTex.SetActive(false);
        
        ResetCamera();
        
        ToggleInputUI(true);
        UpdateDescriptionSafe(0);
    }

    private void ToggleInputUI(bool uiEnabled)
    {
        boundsX.interactable = uiEnabled;
        boundsY.interactable = uiEnabled;
        minCellX.interactable = uiEnabled;
        minCellY.interactable = uiEnabled;
        bspItterations.interactable = uiEnabled;
        enableDLA.interactable = uiEnabled;
        dlaParticleCount.interactable = uiEnabled;

        generateButton.interactable = uiEnabled;
        resetButton.interactable = !uiEnabled;
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
        
        drawnRectangles.Add(lineRender);
    }

    private void UpdateDescriptionSafe(int index)
    {
        //Check we have valid description
        if (descriptions == null || index < 0 || index >= descriptions.Length)
        {
            return;
        }

        Description description = descriptions[index];

        titleField.text = description.title;
        descriptionField.text = description.description;
    }
    
    private void ResetCamera()
    {
        if (!Camera.main)
        {
            return;
        }

        Camera.main.transform.position = camDefaultPos.position;
        Camera.main.transform.rotation = camDefaultPos.rotation;
    }

    private IEnumerator WaitForAdvanceButtonPressIfNoAutoAdvance()
    {
        if (autoAdvance && autoAdvance.isOn)
        {
            yield break;
        }
        
        yield return new ActivateButtonAndWaitForPress(advanceButton);
    }

    private IEnumerator RotateCamForever()
    {
        if (!Camera.main)
        {
            yield break;
        }

        const float rotateSpeed = 10f;
        Camera.main.transform.position = camRotatePos.position;

        while (true)
        {
            Camera.main.gameObject.transform.LookAt(new Vector3(100,0,100));
            Camera.main.gameObject.transform.Translate(Vector3.right * rotateSpeed * Time.deltaTime);

            yield return null;
        }

    }
}
