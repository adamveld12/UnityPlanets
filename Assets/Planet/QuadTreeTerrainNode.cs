using System;
using System.Linq;
using UnityEngine;

namespace Assets.Planet
{
    [Serializable]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class QuadTreeTerrainNode : MonoBehaviour
    {
        private QuadTreeTerrainNode _parent;
        private QuadTreeTerrainNode[] _children;


        private int _depth = 1;
        private float _size;
        private float _surfaceRadius;
        private float _splitDistance;

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
            var go = new GameObject(parent == null ? "parent" : string.Format("level {0}", parent.Depth+1), typeof(QuadTreeTerrainNode))
            {
                isStatic = true
            };

            var qn = go.GetComponent<QuadTreeTerrainNode>();

            qn._children = null;
            qn._size = size;
            qn.SurfaceRadius = size;

            if (parent != null)
            {
                qn._parent = parent;
                qn._depth = parent.Depth + 1;
                qn.transform.parent = parent.transform;
                qn.WorldCenter = parent.WorldCenter;
                qn.SurfaceRadius = parent.SurfaceRadius;
            }


            var meshRenderer = qn.GetComponent<MeshRenderer>();

            meshRenderer.materials = new[] {
              new Material(Shader.Find("Diffuse")), 
            };

            var meshFilter = qn.GetComponent<MeshFilter>();

            if (meshFilter.mesh == null)
                meshFilter.mesh = new Mesh();

            return qn;
        }

        public void Start()
        {
            GenerateModel();
        }
 
        public void Update()
        {
            // if we're the root, we control updates all the way down
            if (Parent == null)
            {
                var camPos = Camera.main.transform.position;
                Update(camPos);
            }
        }

        public void Update(Vector3 cameraPosition)
        {
            // this will need a vastly more complex algorithm, including dot products linear cam distance and other crap
            var diff = transform.position - WorldCenter;
            var length = diff.magnitude;
            var projected = SurfaceRadius/length*diff;
            var distance = (cameraPosition - projected).magnitude;


            _splitDistance = Size/(Depth*2);

            if (Parent == null)
              Debug.LogFormat("Camera Distance: {0}  Next Split: {1}  Size {2}", distance, _splitDistance, Size);


            Debug.DrawRay(projected, (cameraPosition - projected));

            var shouldSplit = distance > 150 && distance < _splitDistance;

            if (!IsLeaf)
                _children.ForEach(x => x.Update(cameraPosition));

            if (IsLeaf && shouldSplit)
                Split();
            else if (!IsLeaf && !shouldSplit)
                Collapse();

        }

        public void GenerateModel()
        {
            if (IsLeaf)
            {
                var meshFilter = GetComponent<MeshFilter>();
                if (meshFilter.mesh == null)
                    meshFilter.mesh = new Mesh();
                var mesh = meshFilter.mesh;

                Vector3[] verts;
                int[] indices;
                GeometryHelper.Patch(Size, out verts, out indices);
                verts = verts.Select(vert =>
                {
                     var diff = transform.TransformPoint(vert - WorldCenter);
                     var length = diff.magnitude;
                     var projected = SurfaceRadius/length*diff;

                    return transform.InverseTransformPoint(projected - WorldCenter);
                }).ToArray();

                mesh.vertices = verts;
                mesh.triangles = indices;
                mesh.RecalculateNormals();
                mesh.Optimize();
                mesh.RecalculateBounds();
            }
        }

        /// <summary>
        /// only split one level per frame
        /// </summary>
        public void Split()
        {
            if (Depth >= 10)
                return;

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
                GetComponent<MeshRenderer>().enabled = false;
                gameObject.transform.SetParent(null, true);
                Destroy(gameObject);
            }
            else
            {
                transform.DetachChildren();
                GetComponent<MeshRenderer>().enabled = true;

                _children.ForEach(node => node.Collapse());
                _children = null;
            }
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
            set {

                if (Math.Abs(_size - value) > float.Epsilon)
                {
                _size = value; 
                GenerateModel();
                    
                }
            }
        }

        public QuadTreeTerrainNode Parent { get { return _parent; }}
        public Vector3 WorldCenter { get; set; }

        public float SurfaceRadius
        {
            get { return _surfaceRadius; }
            set
            {
                if (Math.Abs(_surfaceRadius - value) > float.Epsilon)
                {
                _surfaceRadius = value; 
                    GenerateModel();
                    
                }
            }
        }
    }
}
