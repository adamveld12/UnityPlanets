using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Planet
{
    public class QuadTreeNode
    {
        private Vector3[] _vertsCache;
        private int[] _indicesCache;
        private bool _isLeaf = true;

        private QuadTreeNode[] _children;
        private bool _isDirty;
        private readonly QuadTreeNode _parent;

        public QuadTreeNode(QuadTreeNode parent) : this(parent, new GNodeTransform {Parent = parent.Location}, parent.Size*0.5f) { }

        public QuadTreeNode(Vector3 location, float size) : this(null, new GNodeTransform { Location = location }, size) {}

        private QuadTreeNode(QuadTreeNode parent, GNodeTransform location, float size)
        {
            _isDirty = true;
            _children = null;
            _parent = parent;

            Location = location;
            Size = size;
            Depth = parent?.Depth + 1 ?? 0;
        }

        public void Update(Vector3 camPosition)
        {
            var distance = (Location.TransformVector - camPosition).magnitude;
            
            var splitDistance = Size * Size;
            var shouldSplit = distance < splitDistance;

            if (!IsLeaf)
                ForEachChild(x => x.Update(camPosition));

            if (IsLeaf && shouldSplit)
                Split();
            else if (!IsLeaf && !shouldSplit)
                Collapse();
        }


        public void GenerateModel(out Vector3[] verts, out int[] indices)
        {
            if (IsLeaf && _vertsCache != null)
            {
                verts = _vertsCache;
                indices = _indicesCache;
            }

            if (IsLeaf)
            {
                var location = Location.TransformVector;
                Vector2[] uvs;
                GeometryHelper.Plane(Size, out verts, out indices, out uvs);
                verts = verts.Select(x => new Vector3(x.x + location.x, x.y + location.y, x.z + location.z)).ToArray();
                _vertsCache = verts;
                _indicesCache = indices;
            }
            else 
            {
                var completeVertices = new List<Vector3>(_childCount * 4);
                var completeIndices = new List<int>(_childCount * 6);
                
                for (var i = 0; i < _children.Length; i++)
                {
                    var child = _children[i];

                    Vector3[] childVerts;
                    int[] childIndices;

                    if (child._vertsCache != null && child.IsLeaf)
                    {
                        childVerts = child._vertsCache;
                        childIndices = child._indicesCache;
                    }
                    else
                        child.GenerateModel(out childVerts, out childIndices);

                    completeIndices.AddRange(childIndices.Select(index =>  completeVertices.Count + index));
                    completeVertices.AddRange(childVerts);
                }

                verts = completeVertices.ToArray();
                indices = completeIndices.ToArray();
            }

            _isDirty = false;
        }

        /// <summary>
        /// only split one level per frame
        /// </summary>
        public void Split()
        {
              if (Depth >= 17)
                  return;

            if (_children == null)
            {

                var childSize = Size*0.25f;
                Vector3[] centerPoints = {
                   new Vector3(-childSize, 0, childSize),
                   new Vector3(childSize, 0, childSize),
                   new Vector3(-childSize, 0, -childSize),
                   new Vector3(childSize, 0, -childSize)
                };

                _children = new QuadTreeNode[4];
                for (int i = 0; i < _children.Length; i++)
                {
                    _children[i] = new QuadTreeNode(this) {
                        Location = {
                            Location = centerPoints[i]
                        }
                    };
                }

            }

            _isLeaf = false;
            PropagateChange();
        }

        public void Collapse()
        {
            if (IsLeaf) return;

            _isLeaf = true;
            ForEachChild(node => node.Collapse());
            PropagateChange();
        }

        private void ForEachChild(Action<QuadTreeNode> operation)
        {
            if (operation == null)
                throw new ArgumentNullException();

            foreach (var child in _children)
                operation.Invoke(child);
        }

        private void PropagateChange()
        {
            _isDirty = true;
            _parent?.PropagateChange();
            _childCount = CountChildren();
        }

        private int CountChildren()
        {
            if (IsLeaf)
                return 0;

            return _children.Length + _children.Sum(x => x.CountChildren());
        }

        private int _childCount = 0;
        public float Size { get; }
        public int Depth { get; }
        public bool IsDirty => _isDirty;
        public bool IsLeaf => _isLeaf;
        public GNodeTransform Location { get; set; }
    }
}
