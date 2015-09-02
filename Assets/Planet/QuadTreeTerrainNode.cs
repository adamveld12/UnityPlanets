using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Assets.Planet
{
    [Serializable]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class QuadTreeTerrainNode : MonoBehaviour
    {
        private QuadTreeTerrainNode _parent;
        private QuadTreeTerrainNode[] _children;

        private bool _isLeaf = true;
        private bool _dead;

        private int _depth = 1;
        private float _size;

        public static QuadTreeTerrainNode CreateParentNode(float size)
        {
            return CreateNode(null, size);
        }

        public static QuadTreeTerrainNode CreateChildNode(QuadTreeTerrainNode parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            return CreateNode(parent, parent.Size*0.5f);
        }

        private static QuadTreeTerrainNode CreateNode(QuadTreeTerrainNode parent, float size)
        {
            var go = new GameObject(parent == null ? "parent" : string.Format("level {0}", parent.Depth+1), typeof(QuadTreeTerrainNode)) {
            };

            var qn = go.GetComponent<QuadTreeTerrainNode>();

            qn._children = null;
            qn._size = size;

            if (parent != null)
            {
                qn._parent = parent;
                qn._depth = parent.Depth + 1;
                qn.transform.parent = parent.transform;
            }

            var meshFilter = qn.GetComponent<MeshFilter>();

            if (meshFilter.mesh == null)
                meshFilter.mesh = new Mesh();

            return qn;
        }

        public void Start()
        {
        }
 
        public void Update()
        {
            // if we're the root, we control updates all the way down
            if (Parent == null)
            {
                var camPos = Camera.main.transform.position;
                Update(camPos);
            }

            if (!IsLeaf)
            {
                var parentCenter = transform.localPosition;
                var topLeft = new Vector3(parentCenter.x - Size*0.5f, parentCenter.y + 10, parentCenter.z - Size * 0.5f);
                var bottomRight = new Vector3(parentCenter.x + Size*0.5f, parentCenter.y + 10, parentCenter.z + Size * 0.5f);

                Debug.DrawLine(transform.TransformPoint(topLeft), transform.TransformPoint(bottomRight), Color.green);
            }
        }

        public void Update(Vector3 cameraPosition)
        {
            // this will need a vastly more complex algorithm, including dot products linear cam distance and other crap
            var distance = (transform.position - cameraPosition).magnitude;
            var splitDistance = Size;
            var shouldSplit = distance < splitDistance;

            //transform.localRotation.Set(0, 0, 0, 1);

            if (!IsLeaf)
              ForEachChild(x => x.Update(cameraPosition));

            if (IsLeaf && shouldSplit)
                Split();
            else if (!IsLeaf && !shouldSplit)
                Collapse();
        }

        public void GenerateModel()
        {
            if (IsLeaf)
            {
                Vector3[] verts;
                int[] indices;
                GeometryHelper.Patch(Size, out verts, out indices);

                var meshFilter = GetComponent<MeshFilter>();

                if (meshFilter.mesh == null)
                    meshFilter.mesh = new Mesh();

                var mesh = meshFilter.mesh;

                mesh.vertices = verts;
                mesh.triangles = indices;
                mesh.RecalculateNormals();
                mesh.Optimize();
            }
        }

        /// <summary>
        /// only split one level per frame
        /// </summary>
        public void Split()
        {
            if (Depth >= 10)
                return;

            _isLeaf = false;
            GetComponent<MeshRenderer>().enabled = false;

            var childOffset = Size*0.25f;
            Vector3[] centerPoints = {
               new Vector3(-childOffset, 0, childOffset),
               new Vector3(childOffset, 0, childOffset),
               new Vector3(-childOffset, 0, -childOffset),
               new Vector3(childOffset, 0, -childOffset)
            };

            if (_children == null)
                _children = new QuadTreeTerrainNode[4];


            for (var i = 0; i < _children.Length; i++)
            {
                var child = _children[i] = CreateChildNode(this);
                child.transform.localPosition = centerPoints[i];
                child.transform.localRotation = new Quaternion(0, 0, 0, 1);
                child.GenerateModel();
            }
        }

        public void Collapse()
        {
            if (IsLeaf)
            {
                _parent = null;
                _dead = true;

                GetComponent<MeshRenderer>().enabled = false;
                gameObject.transform.SetParent(null, true);
                Destroy(gameObject);
            }
            else
            {
                transform.DetachChildren();
                GetComponent<MeshRenderer>().enabled = true;

                ForEachChild(node => node.Collapse());
                _children = null;
            }
        }

        private void ForEachChild(Action<QuadTreeTerrainNode> operation)
        {
            if (operation == null)
                throw new ArgumentNullException();

            foreach (var child in _children)
                operation.Invoke(child);
        }

        public int Depth
        {
            get { return _depth; }
        }

        public bool IsLeaf
        {
            get { return _children == null; }
        }

        public float Size
        {
            get { return _size; }
        }

        public QuadTreeTerrainNode Parent { get { return _parent; }}
    }
}
