using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Planet
{
    public class QuadTreeNode
    {
        private QuadTreeNode[] _children;
        private QuadTreeNode _parent;

        public QuadTreeNode(QuadTreeNode parent) : this(parent, parent.Size/2) { }
        public QuadTreeNode(float size) : this(null, size) { }

        private QuadTreeNode(QuadTreeNode parent, float size)
        {
            _parent = parent;
            IsDirty = true;
            Size = size;
            Depth = _parent == null ? 0 : _parent.Depth + 1;
        }

        public void Update(Vector3 camPosition)
        {
            var shouldSplit = false;
            if (IsLeaf && shouldSplit)
                Split();
        }

        public List<Vector3> GenerateModel()
        {
            List<Vector3> model = new List<Vector3>();
            if (IsLeaf)
            {

               
            }
            else
            {
                foreach (var child in _children)
                    model.AddRange(child.GenerateModel());
            }

            return model;
        }

        private void Generate()
        {
        }

        /// <summary>
        /// only split one level per frame
        /// </summary>
        public void Split()
        {
            if (IsLeaf)
            {
                if (Depth >= MaxSplitDepth)
                    return;

                IsDirty = false;


                Debug.Log("updating mesh to be invisible");

                var myPos = transform.localPosition;
                _children = new[] {
                 new QuadTreeNode(this), 
                 new QuadTreeNode(this), 
                 new QuadTreeNode(this), 
                 new QuadTreeNode(this), 
               };

                foreach (var terrain in _children)
                {
                    //            var local1 = terrain.transform.localPosition;
                    //            local1.x = childSize*0.5f;
                    //            local1.z = childSize*0.5f;
                    //            terrain.transform.localPosition = local1;
                }

                //        var local = _children[1].transform.localPosition;
                //        local.x = -childSize*0.5f;
                //        _children[1].transform.localPosition = local;
                //
                //        local = _children[2].transform.localPosition;
                //        local.z = -childSize*0.5f;
                //        _children[2].transform.localPosition = local;
                //
                //        local = _children[3].transform.localPosition;
                //        local.x = -childSize*0.5f;
                //        local.z = -childSize*0.5f;
                //        _children[3].transform.localPosition = local;
            }
        }

        public void Collapse()
        {
            if (!IsLeaf)
            {
                ForEachChild(node => {
                    node.Collapse();
                });

                _children = null;
                IsDirty = true;
            }
        }

        private void ForEachChild(Action<QuadTreeNode> operation)
        {
            if (operation == null)
                throw new ArgumentNullException();

            foreach (var child in _children)
                operation.Invoke(child);
        }

        public float Size { get; private set; }
        public int Depth { get; private set; }
        public bool IsDirty { get; private set; }
        public bool IsLeaf
        {
            get { return _children == null; }
        }
    }
}
