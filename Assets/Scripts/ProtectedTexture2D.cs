
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper class for Texture 2D that allows the certiain cells to be protected
/// </summary>
public class ProtectedTexture2D
{
    public Texture2D texture;
    private List<Vector2Int> protectedPixels;

    public ProtectedTexture2D(int width, int height, FilterMode filterMode = FilterMode.Point)
    {
        texture = new Texture2D(width, height);
        texture.filterMode = filterMode;

        protectedPixels = new List<Vector2Int>(2);
    }

    ~ProtectedTexture2D()
    {
        //Clear mem of texture
        texture = null;
    }

    public void ProtectPixel(Vector2Int pos)
    {
        if (!protectedPixels.Contains(pos))
        {
            protectedPixels.Add(pos);
        }
    }

    public void SetPixel(int x, int y, Color color)
    {
        //Check we are not writing to a protected pixel
        if (protectedPixels.Contains(new Vector2Int(x, y)))
        {
            return;
        }
        
        texture.SetPixel(x,y, color);
    }

    public void Apply()
    {
        texture.Apply();
    }
}
