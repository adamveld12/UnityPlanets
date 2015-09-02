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
        public float SurfaceRadius = 512;

        private QuadTreeTerrainNode _root;

        void Start()
        {
            _root = QuadTreeTerrainNode.CreateParentNode(SurfaceRadius);
            _root.transform.parent = transform;
            _root.transform.Translate(new Vector3(0, SurfaceRadius, 0), Space.Self);
            _root.WorldCenter = transform.position;
            _root.SurfaceRadius = SurfaceRadius;
        }

        void Update()
        {
            _root.Size = Size;
            _root.SurfaceRadius = SurfaceRadius;
        }
    }
}