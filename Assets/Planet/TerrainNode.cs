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

        private QuadTreeTerrainNode _root;

        void Start()
        {
            _root = QuadTreeTerrainNode.CreateParentNode(Size);
            _root.transform.parent = transform;
        }

        void Update()
        {
        }
    }
}