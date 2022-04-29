
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Dykstra Pathfinder
/// </summary>
public class Pathfinder
{
    private DijkstraMap map;

    public List<Vector2Int> route;
    
    public bool IsFinished { private set; get; }

    public void Initialize(DijkstraMap map)
    {
        this.map = map;
        route = new List<Vector2Int>();
    }

    /// <summary>
    /// Plot a path from the end of the map to the start of the map
    /// </summary>
    /// <param name="dest"></param>
    public void PlotPathToStartOfMap(Vector2Int dest)
    {
        if (map == null)
        {
            Debug.LogError("Invalid Map");
            return;
        }
        
        Vector2Int currentPos = dest;

        while (currentPos != map.StartPos)
        {

            route.Add(currentPos);
            
            
            List<Vector2Int?> neighbours = map.FindNeighbours(currentPos, true);
            float lowestDistance = float.PositiveInfinity;
            Vector2Int? bestNeighbour = null;
            foreach (Vector2Int? neighbour in neighbours)
            {
                if (neighbour == null)
                {
                    continue;
                }

                float mapDistance = map.distances[neighbour.Value.x, neighbour.Value.y];    
                if (mapDistance < lowestDistance)
                {
                    lowestDistance = mapDistance;
                    bestNeighbour = neighbour;
                }
            }

            if (bestNeighbour == null)
            {
                Debug.Log("Problem tracing path back.");
                break;
            }
            
            currentPos = bestNeighbour.Value;
        }
        
        Debug.Log("done");
        IsFinished = true;
    }

    public void AddPathToTexture(ref Texture2D tex)
    {
        foreach (Vector2Int point in route)
        {
            tex.SetPixel(point.x, point.y, Color.blue);
        }

        tex.Apply();
    }

}
