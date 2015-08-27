using System;
using System.Linq;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class QuadTreeTerrain : MonoBehaviour
{
    [SerializeField]
    public Vector3 Normal;

    private QuadTreeTerrain _parent;
    private QuadTreeTerrain[] _children = null;
    private bool isDirty = true;
    private int depth = 1;

    private Planet _planetParent;


    [SerializeField]
    public int MaxSplitDepth = 1;
    [SerializeField]
    public float SizeInUnits = 256.0f;
    [SerializeField]
    public Texture2D Heightmap;

    // Use this for initialization
    void Start()
    {
        _planetParent = GetComponentInParent<Planet>();
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
            Debug.Log("Max Depth: " +  max + " lodLevel for max: " + lodLevel);
        }

        if (lodLevel > 0.5f && _children == null && depth < MaxSplitDepth)
          Split();
        else if (lodLevel < 0.5f && _children != null) {
          Collapse();
        } else if(isDirty)
          GenerateGeometry();
    }

    private int MaxDepth()
    {
        return _children != null ? _children.Max(x => x.MaxDepth()) : depth;
    }

    private void GenerateGeometry()
    {
        Debug.Log("generating a terrain chunk facing" + Normal);

        Vector3[] verts;
        Vector2[] uvs;
        int[] indices;
        GeometryHelper.Plane(SizeInUnits, out verts, out indices, out uvs);

        verts = verts.Select((vert, index) => vert).ToArray();

        var mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = verts;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.UploadMeshData(false);

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


        isDirty = false;
        Debug.Log("updating mesh to be invisible");
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

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
            terrain.isDirty = true;
            terrain._parent = this;
            terrain.SizeInUnits = childSize;
            terrain.depth = newDepth;
            terrain.transform.parent = transform;
            var local1 = terrain.transform.localPosition;
            local1.x = childSize*0.5f;
            local1.z = childSize*0.5f;
            terrain.transform.localPosition = local1;
        }

        var local = _children[1].transform.localPosition;
        local.x = -childSize*0.5f;
        _children[1].transform.localPosition = local;

        local = _children[2].transform.localPosition;
        local.z = -childSize*0.5f;
        _children[2].transform.localPosition = local;

        local = _children[3].transform.localPosition;
        local.x = -childSize*0.5f;
        local.z = -childSize*0.5f;
        _children[3].transform.localPosition = local;
    }
}