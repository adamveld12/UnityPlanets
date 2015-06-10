using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class QuadTreeTerrain : MonoBehaviour
{
    private QuadTreeTerrainNode _parent;

    private Vector3 _location;

    public int MaxSplitDepth = 1;
    public float SizeInUnits = 256.0f;
    public bool PlanetaryMode = false;

    // Use this for initialization
    void Start()
    {
        _parent = new QuadTreeTerrainNode(MaxSplitDepth, SizeInUnits, transform);

        if (PlanetaryMode)
        {
            var parentTransform = gameObject.transform.parent;
            if (parentTransform == null)
            {
                var exp = new InvalidOperationException("You cannot run the quad tree terrain in planetary mode without attaching to another game object.");
                Debug.LogException(exp);
                throw exp;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        var camPosition = Camera.main.transform.position;

        _parent.Update(camPosition);

        if (_parent.IsDirty)
        {
            Vector3[] verts;
            int[] indices;
            Vector2[] uvs;
            _parent.GenerateGeometry(out verts, out indices, out uvs);

            var meshFilter = GetComponent<MeshFilter>();

            if (meshFilter.mesh == null)
                meshFilter.mesh = new Mesh();

            var mesh = meshFilter.mesh;

            mesh.vertices = verts;
            mesh.triangles = indices;
            mesh.uv = uvs;
        }
    }


}

public static class GeometryHelper
{
    public static void Plane(float sizeInUnits, out Vector3[] verts, out int[] indices, out Vector2[] uvs)
    {
        var halfUnits = sizeInUnits*0.5f;
        verts = new[]
        {
            new Vector3(-halfUnits, 0, -halfUnits), 
            new Vector3(halfUnits, 0, -halfUnits), 

            new Vector3(halfUnits, 0, halfUnits), 
            new Vector3(-halfUnits, 0, halfUnits), 
        };

        indices = new[] {
            3, 1, 0,
            3, 2, 1
        };

        uvs = new[] {
            Vector2.zero,
            new Vector2(1, 0),
            Vector2.one,
            new Vector2(0, 1), 
        };
    }

}