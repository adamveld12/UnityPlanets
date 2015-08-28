using System;
using UnityEngine;

namespace Assets.Planet
{
    [Serializable]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class TerrainNode : MonoBehaviour
    {
        [Tooltip("Size of node in meters.")]
        [SerializeField]
        public int Size = 512;

        [Tooltip("Surface height. If zero then spherical projection does not happen AKA normal flat terrain.")]
        [SerializeField]
        public float SurfaceRadius = 0;

        private QuadTreeNode _root;

        private bool _init;

        void Start()
        {
            _root = new QuadTreeNode(transform.position, Size);
            _init = true;
        }

        void Update()
        {
            if (!_init) return;

            Debug.Assert(_root != null);

            var camPosition = Camera.main.transform;

            _root.Location.Location = transform.position;
            _root.Update(camPosition.position);

            if (_root.IsDirty)
            {
                Vector3[] verts;
                int[] indices;

                _root.GenerateModel(out verts, out indices);

                var mesh = new Mesh {
                    vertices = verts,
                    triangles = indices
                };

                mesh.RecalculateNormals();

                GetComponent<MeshFilter>().mesh = mesh;
            }
        }
    }
}