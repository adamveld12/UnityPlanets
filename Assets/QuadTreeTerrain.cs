using UnityEngine;
using System.Collections;
using System.Linq;

public class QuadTreeTerrain : MonoBehaviour
{
    private QuadTreeTerrain _parent;
    private QuadTreeTerrain[] _children = null;
    private Vector3 _location;
    private bool isDirty = true;
    private int depth = 1;

    public int MaxSplitDepth = 1;
    public float SizeInUnits = 256.0f;

    // Use this for initialization
    void Start()
    {
        GenerateGeometry();
    }

    // Update is called once per frame
    void Update()
    {
        var camPosition = Camera.main.transform.position;
        var ourPosition = transform.position;

        var distance = (ourPosition - camPosition).sqrMagnitude;

        var lodLevel = Mathf.Clamp((depth * (10000.0f / depth))/distance, 0.1f, 1.0f);

        if (_parent == null)
        {
            var max = MaxDepth();
            Debug.Log("Max Depth: " +  max + " lodLevel for max: " + Mathf.Clamp((max * (10000.0f / max))/distance, 0.1f, 1.0f));
        }

        if (lodLevel > 0.5f && _children == null && depth < MaxSplitDepth)
            Split();
        else if (lodLevel < 0.5f && _children != null)
            Collapse();

        if(isDirty)
            GenerateGeometry();
    }

    public void UpdateChildren()
    {

    }

    private int MaxDepth()
    {
        return _children != null ? _children.Max(x => x.MaxDepth()) : depth;
    }

    private void GenerateGeometry()
    {
        var mesh = GetComponent<MeshFilter>().mesh;

        if (mesh == null)
        {
             GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            Debug.Log("mesh prop on mesh filter is null.");
        }

        Vector3[] verts;
        Vector2[] uvs;
        int[] indices;
        GeometryHelper.Plane(SizeInUnits, out verts, out indices, out uvs);


        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = indices;

        isDirty = false;
    }

    private void Collapse()
    {
        if (_children != null)
        {
            foreach (var quadTreeNode in _children)
            {
                quadTreeNode.Collapse();
                Destroy(quadTreeNode.gameObject);
            }

            _children = null;

            isDirty = true;
        }
    }

    private void Split()
    {
        if (depth >= MaxSplitDepth)
            return;

        GetComponent<MeshFilter>().mesh = null;

        var myPos = transform.localPosition;
        _children = new[] {
           (QuadTreeTerrain) Instantiate(this, myPos, transform.rotation),
           (QuadTreeTerrain) Instantiate(this, myPos, transform.rotation),
           (QuadTreeTerrain) Instantiate(this, myPos, transform.rotation),
           (QuadTreeTerrain) Instantiate(this, myPos, transform.rotation),
       };

        var childSize = SizeInUnits*0.5f;
        var newDepth = depth + 1;

        foreach (var terrain in _children)
        {
            terrain._parent = this;
            terrain.SizeInUnits = childSize;
            terrain.depth = newDepth;
            terrain.transform.parent = transform;
            var local1 = terrain.transform.localPosition;
            terrain.transform.localPosition = local1;
        }

        var local = _children[0].transform.localPosition;
        local.x = childSize*0.5f;
        local.z = childSize*0.5f;
        _children[0].transform.localPosition = local;

        local = _children[1].transform.localPosition;
        local.x = -childSize*0.5f;
        local.z = childSize*0.5f;
        _children[1].transform.localPosition = local;

        local = _children[2].transform.localPosition;
        local.x = childSize*0.5f;
        local.z = -childSize*0.5f;
        _children[2].transform.localPosition = local;

        local = _children[3].transform.localPosition;
        local.x = -childSize*0.5f;
        local.z = -childSize*0.5f;
        _children[3].transform.localPosition = local;
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


