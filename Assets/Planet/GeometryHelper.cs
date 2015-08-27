using UnityEngine;

public static class GeometryHelper
{
    public static void Triangle(float sizeInUnits, out Vector3[] verts, out int[] indices, out Vector2[] uvs)
    {
        var halfUnits = sizeInUnits*0.5f;
        verts = new[] {
            new Vector3(-halfUnits, 0, -halfUnits), 
            new Vector3(halfUnits, 0, -halfUnits), 
            new Vector3(halfUnits, 0, halfUnits), 
        };

        indices = new[] {
            0, 1, 2
        };

        uvs = new[] {
            Vector2.zero,
            new Vector2(1, 0),
            Vector2.one,
            new Vector2(0, 1), 
        };
        
    }
    public static void Plane(float sizeInUnits, out Vector3[] verts, out int[] indices, out Vector2[] uvs)
    {
        var halfUnits = sizeInUnits*0.5f;
        verts = new[] {
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