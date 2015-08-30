using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Planet
{
    public class QuadTreeNode
    {
        private MeshDef _meshCache;

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
            
            var splitDistance = Size;
            var shouldSplit = distance < splitDistance;

            if (!IsLeaf)
                ForEachChild(x => x.Update(camPosition));

            if (IsLeaf && shouldSplit)
                Split();
            else if (!IsLeaf && !shouldSplit)
                Collapse();
        }


        public IEnumerable<MeshDef> GenerateModel()
        {
            _isDirty = false;
            if (IsLeaf && _meshCache != null)
            {
               yield return _meshCache;
               yield break;
            }

            if (IsLeaf)
            {
                var location = Location.TransformVector;
                Vector3[] verts;
                int[] indices;
                GeometryHelper.Patch(Size, out verts, out indices);
                verts = verts.Select(x => new Vector3(x.x + location.x, x.y + location.y, x.z + location.z)).ToArray();
                _meshCache = new MeshDef(verts, indices);
                yield return _meshCache;
            }
            else 
            {
                for (var i = 0; i < _children.Length; i++)
                {
                    var child = _children[i];

                    foreach (var meshDef in child.GenerateModel())
                        yield return meshDef;
                }
            }
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
