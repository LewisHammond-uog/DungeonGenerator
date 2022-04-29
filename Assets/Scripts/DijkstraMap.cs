
using System.Collections;
using System.Collections.Generic;
using FloatExtensions;
using UnityEditor;
using UnityEngine;

public class DijkstraMap
{
    //Map of cells
    private Vector2Int gridSize;
    private GameObject[,] cells;
    
    //Dijkstra arrays
    private float[,] distances; //distances from start point
    private HashSet<Vector2Int> tentativesSet;
    private HashSet<Vector2Int> unvisitedSet;

    private Vector2Int currentPos;

    //Max distance in this map
    private float maxDistance;

    public bool finished;


    public void Initialize(Vector2Int start, GameObject[,] cells)
    {
        if (cells == null || cells.Length == 0)
        {
            Debug.LogError("Cells are invalid");
            return;
        }

        gridSize = new Vector2Int(cells.GetLength(0), cells.GetLength(1));
        this.cells = cells;

        distances = new float[gridSize.x, gridSize.y];
        tentativesSet = new HashSet<Vector2Int>();
        unvisitedSet = new HashSet<Vector2Int>();
        
        InitializeDistances();
        InitializeUnvisitedSet();

        currentPos = start;
        distances[start.x, start.y] = 0f;

        maxDistance = 0f;

        finished = false;
    }

    public void Update()
    {
        if (!finished)
        {
            FillDijkstraMap();
        }
    }
    
    public void FillDijkstraMap()
    {
        float currentPosDist = distances[currentPos.x, currentPos.y];
        
        //Get neighbours of this node
        List<Vector2Int?> currentNeighbours = FindNeighbours(currentPos);
        foreach (Vector2Int? neighbour in currentNeighbours)
        {
            if (neighbour != null)
            {
                tentativesSet.Add((Vector2Int)neighbour);
            }
        }
        
        //Update distances of all neighbours
        foreach (Vector2Int tentative in tentativesSet)
        {
            float tentativeDist = distances[tentative.x, tentative.y];
            tentativeDist = currentPosDist + 1 > tentativeDist ? tentativeDist : currentPosDist + 1;
            distances[tentative.x, tentative.y] = tentativeDist;

            if (tentativeDist > maxDistance)
            {
                maxDistance = tentativeDist;
            }
        }
        
        //Remove current pos from the tentative and unvisited set
        unvisitedSet.Remove(currentPos);
        tentativesSet.Remove(currentPos);
        
        //Find the neighbour with the closest distance
        float closestDist = float.PositiveInfinity;
        foreach (Vector2Int tenative in tentativesSet)
        {
            float dist = distances[tenative.x, tenative.y];
            
            if (dist < closestDist)
            {
                closestDist = dist;
                currentPos = tenative;
            }
        }

        if (tentativesSet.Count == 0)
        {
            finished = true;
        }
    }

    public Texture2D GetMapAsTexture()
    {
        Texture2D tex = new Texture2D(gridSize.x, gridSize.y);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                
                float distance = distances[x, y];

                if (distance == 0f)
                {
                    tex.SetPixel(x,y,new Color(0,1,0,1));
                    continue;
                }
                
                if (float.IsPositiveInfinity(distance))
                {
                    tex.SetPixel(x,y, new Color(0,0,0,0));
                }
                else
                {
                    float texValue = distance.Remap(0, maxDistance, 1, 0);
                    tex.SetPixel(x,y, new Color(0,0,texValue,1));
                }
            }
        }
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// Find neighbour gameobjects of a given position
    /// </summary>
    /// <param name="pos"></param>
    private List<Vector2Int?> FindNeighbours(Vector2Int pos, bool ignoreUnvisitedSet = false)
    {
        List<Vector2Int?> neighboursList = new List<Vector2Int?>
        {
            //Get the neighbours in the cardinal NESW directions
            GetNeighbour(pos, Vector2Int.left),
            GetNeighbour(pos, Vector2Int.right),
            GetNeighbour(pos, Vector2Int.up),
            GetNeighbour(pos, Vector2Int.down)
        };

        return neighboursList;
    }

    private Vector2Int? GetNeighbour(Vector2Int pos, Vector2Int direction, bool ignoreUnvisitedSet = false)
    {
        Vector2Int neighbourPos = pos + direction;
        
        //Bounds check
        if (neighbourPos.x < 0 || neighbourPos.x >= gridSize.x
                               || neighbourPos.y < 0 || neighbourPos.y >= gridSize.y)
        {
            return null;
        }

        //Don't include if not in the unvisited set
        if (!unvisitedSet.Contains(neighbourPos) && !ignoreUnvisitedSet)
        {
            return null;
        }
        
        return neighbourPos;
    }

    /// <summary>
    /// Set all distances to be infinity
    /// </summary>
    private void InitializeDistances()
    {
        for (int x = 0; x< gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                //Set all distances that we haven't calculated yet to infinity
                distances[x, y] = float.PositiveInfinity;
            }
        }
    }
    
    /// <summary>
    /// Add all valid cells to the unvisited set
    /// </summary>
    private void InitializeUnvisitedSet()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (cells[x, y] != null)
                {
                    unvisitedSet.Add(new Vector2Int(x,y));
                }
            }
        }
    }
}
