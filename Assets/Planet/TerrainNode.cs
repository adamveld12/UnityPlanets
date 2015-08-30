using System;
using System.Collections.Generic;
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
        private List<GameObject> _gameObjects = new List<GameObject>();

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
                _gameObjects.ForEach(DestroyImmediate);
                _gameObjects.Clear();

                int meshId = 0;
                var meshDefs = _root.GenerateModel();
                foreach (var meshDef in meshDefs)
                {
                    var go = new GameObject($"terrain sub node {meshId++}");
                    go.AddComponent<MeshFilter>().mesh = meshDef.ToMesh();
                    go.AddComponent<MeshRenderer>();

                    _gameObjects.Add(go);
                }
            }
        }
    }
}