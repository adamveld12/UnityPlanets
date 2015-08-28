using System;
using UnityEngine;

namespace Assets.Planet
{
    [Serializable]
    public class Planet : MonoBehaviour
    {
        private QuadTreeNode[] _terrain;
        //private QuadTreeNode[] _sky;

        [SerializeField]
        [Tooltip("Sea level altitude from the center of the planet in meters")]
        public float SurfaceRadius = 128.0f;

        [SerializeField]
        [Tooltip("The end of the atmosphere in kilometers. 0 =< sea level")]
        public float AtmosphereAltitude = 0f;


        // Use this for initialization
        void Start()
        {
            // create 6 quadtree terrains, 1 for each side of the cube
            // tell them to be at a radius = surface radius
            // make 6 more quad tree terrains, making their radius = atmosphereAlititude + surface radius for atmo scattering stuff

            var terrainHeight = SurfaceRadius*2;
            Debug.Log($"Setting up terrain at {terrainHeight}");

            var pos = transform.position;
            _terrain = new[]{
                new QuadTreeNode(pos, terrainHeight), 
                new QuadTreeNode(pos, terrainHeight), 
                new QuadTreeNode(pos, terrainHeight), 
                new QuadTreeNode(pos, terrainHeight), 
                new QuadTreeNode(pos, terrainHeight), 
                new QuadTreeNode(pos, terrainHeight), 
            };

            // if the atmo is at/below zero then we just don't render an atmo at all
            if (AtmosphereAltitude >= 0)
            {
                var skyHeight = SurfaceRadius*2 + AtmosphereAltitude;
                Debug.Log($"Setting up sky at {skyHeight}");


//                _sky = new[]{
//                    new QuadTreeNode(skyHeight), 
//                    new QuadTreeNode(skyHeight), 
//                    new QuadTreeNode(skyHeight), 
//                    new QuadTreeNode(skyHeight), 
//                    new QuadTreeNode(skyHeight), 
//                    new QuadTreeNode(skyHeight)
//                };
            }

        }

        private void UpdateTerrain()
        {
            var transform = Camera.main.transform;

            if (_terrain != null)
            {
              foreach (var face in _terrain)
                  face.Update(transform.position);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // when camera is far enough away, just hide the planet all together
            UpdateTerrain();
        }
    }
}
