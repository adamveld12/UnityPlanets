using System;
using UnityEngine;

namespace Assets.Planet
{
    [Serializable]
    public class Planet : MonoBehaviour
    {

        [SerializeField]
        [Tooltip("Sea level altitude from the center of the planet in meters")]
        public float SurfaceRadius = 128.0f;

        [SerializeField]
        [Tooltip("The end of the atmosphere in kilometers. 0 =< sea level")]
        public float AtmosphereAltitude;


        private QuadTreeTerrainNode[] _surfaceNodes;

        void Reset()
        {
            SurfaceRadius = 128.0f;
            AtmosphereAltitude = 0;

        }


        // Use this for initialization
        void Start()
        {
            InitializePlanetQuads();

            // if the atmo is at/below zero then we just don't render an atmo at all
            if (AtmosphereAltitude >= 0)
            {
                var skyHeight = SurfaceRadius*2 + AtmosphereAltitude;
                Debug.LogFormat("Setting up sky at {0}", skyHeight);


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

        private void InitializePlanetQuads()
        {
            // create 6 quadtree terrains, 1 for each side of the cube
            // tell them to be at a radius = surface radius
            // make 6 more quad tree terrains, making their radius = atmosphereAlititude + surface radius for atmo scattering stuff

            var size = SurfaceRadius*2;
            _surfaceNodes = new[]
            {
                QuadTreeTerrainNode.CreateParentNode(size),
                QuadTreeTerrainNode.CreateParentNode(size),
                QuadTreeTerrainNode.CreateParentNode(size),
                QuadTreeTerrainNode.CreateParentNode(size),
                QuadTreeTerrainNode.CreateParentNode(size),
                QuadTreeTerrainNode.CreateParentNode(size),
            };

            var normals = new[]
            {
                Vector3.up,
                Vector3.down,
                Vector3.back,
                Vector3.forward,
                Vector3.left,
                Vector3.right
            };

            _surfaceNodes.ForEach((node, index) =>
            {
                node.transform.parent = transform;

                var normal = normals[index];
                node.transform.localPosition = SurfaceRadius*normal;
                node.WorldCenter = transform.position;
                node.SurfaceRadius = SurfaceRadius;
            });

            var s = _surfaceNodes;
            s[0].transform.localEulerAngles.Set(0, 0, 0);
            s[1].transform.localRotation = Quaternion.Euler(180, 0, 0);
            s[2].transform.localRotation = Quaternion.Euler(270, 0, 0);
            s[3].transform.localRotation = Quaternion.Euler(90, 0, 0);
            s[4].transform.localRotation = Quaternion.Euler(0, 0, 90);
            s[5].transform.localRotation = Quaternion.Euler(0, 0, 270);
        }


        // Update is called once per frame
        void Update()
        {
        }
    }
}
