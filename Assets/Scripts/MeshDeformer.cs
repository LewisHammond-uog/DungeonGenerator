using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshDeformer : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;
    private List<Vector3> verts;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        verts = mesh.vertices.ToList();
    }
    
    public void DeformMesh(Vector3 point, float radius, float stepRadius, float strength, float stepStrength, Vector3 direction)
    {
        for (int i = 0; i < verts.Count; i++)
        {
            Vector3 vert = verts[i];
            Vector3 vi = transform.TransformPoint(vert);
            //Get distance from hit point to the vert
            float distance = Vector3.Distance(point, vi);
            float s = strength;
            for (float r = 0; r < radius; r += stepRadius)
            {
                if (distance < r)
                {
                    Vector3 deformation = direction * s;
                    verts[i] = transform.InverseTransformPoint(vi + deformation);
                }

                s -= stepStrength;
            }
        }
        
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
        mesh.SetVertices(verts);
    }
}
