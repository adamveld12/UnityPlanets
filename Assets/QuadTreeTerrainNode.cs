using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadTreeTerrainNode
{
    private readonly QuadTreeTerrainNode _parent;
    private readonly int _maxDepth;
    private readonly float _sizeInUnits;
    private readonly int _depth = 1;

    private QuadTreeTerrainNode[] _children;
    private bool _isDirty = true;
    private Vector3 _transform;
    
    public QuadTreeTerrainNode(QuadTreeTerrainNode parent, int depth, int maxDepth, float sizeInUnits) : this(maxDepth, sizeInUnits, null)
    {
        if(parent == null)
            throw new ArgumentNullException();

        _parent = parent;
        _depth = depth;
    }

    public QuadTreeTerrainNode(int maxDepth, float sizeInUnits, Vector3 location = Vector3.zero)
    {
        _sizeInUnits = sizeInUnits;
        _maxDepth = maxDepth;

        _transform = location;

    }

    public void Update(Vector3 camPosition)
    {
        var ourPosition = _transform;

        var distance = (ourPosition - camPosition).sqrMagnitude;

        var lodLevel = Mathf.Clamp((_depth * (10000.0f / _depth))/distance, 0.1f, 1.0f);

        if (_parent == null)
        {
            var max = MaxDepth();
            Debug.Log("Max Depth: " +  max + " lodLevel for max: " + Mathf.Clamp((max * (10000.0f / max))/distance, 0.1f, 1.0f));
        }

        if (lodLevel > 0.5f && _children == null && _depth < _maxDepth)
            Split();
        else if (lodLevel < 0.5f && _children != null)
            Collapse();

    }

    public void Split()
    {
        if (_depth >= _maxDepth)
            return;

        var newDepth = _depth + 1;
        var childSize = SizeInUnits*0.5f;

        _children = new[] {
            new QuadTreeTerrainNode(this, newDepth, _maxDepth, childSize), 
            new QuadTreeTerrainNode(this, newDepth, _maxDepth, childSize), 
            new QuadTreeTerrainNode(this, newDepth, _maxDepth, childSize), 
            new QuadTreeTerrainNode(this, newDepth, _maxDepth, childSize), 
       };

        foreach (var terrain in _children)
        {
            var txfm = terrain._transform;
            txfm.x = childSize*0.5f;
            txfm.z = childSize*0.5f;
            terrain._transform = txfm;
        }

        var local = _children[1]._transform.localPosition;
        local.x = -childSize*0.5f;
        _children[1]._transform.localPosition = local;

        local = _children[2]._transform.localPosition;
        local.z = -childSize*0.5f;
        _children[2]._transform.localPosition = local;

        local = _children[3]._transform.localPosition;
        local.x = local.z = -childSize*0.5f;
        _children[3]._transform.localPosition = local;
    }

    public void GenerateGeometry(out Vector3[] verts, out int[] tris, out Vector2[] uvs)
    {

        if (_children != null)
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var uvsCoords = new List<Vector2>();

            foreach (var child in _children)
            {
                Vector3[] localVerts;
                int[] localTris;
                Vector2[] localUvs;

                child.GenerateGeometry(out localVerts, out localTris, out localUvs);

                var vertCount = vertices.Count;
                indices.AddRange(localTris.Select(x => x + vertCount));
                vertices.AddRange(localVerts.Select(x => _transform.TransformVector(x)));
                uvsCoords.AddRange(localUvs);
            }

            verts = vertices.ToArray();
            tris = indices.ToArray();
            uvs = uvsCoords.ToArray();
        }
        else
            GeometryHelper.Plane(SizeInUnits, out verts, out tris, out uvs);
        
        _isDirty = false;
    }

    private void Collapse()
    {
        if (_children != null)
        {
            _children = null;
            _isDirty = true;
        }
    }

    private int MaxDepth()
    {
        return _children != null ? _children.Max(x => x.MaxDepth()) : _depth;
    }

    public int Depth
    {
        get { return _depth; }
    }

    public bool IsDirty
    {
        get { return _isDirty;  }
    }

    public float SizeInUnits
    {
        get { return _sizeInUnits; }
    } 
}