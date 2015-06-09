using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {
    private GameObject[] _sides = new GameObject[6];
    public float SurfaceRadius = 128.0f;
    public float AtmosphereAltitude = 128.0f;

    // Use this for initialization
    void Start () {
      // create 6 quadtree terrains, 1 for each side of the cube
      // tell them to be at a radius = surface radius
      // make 6 more quad tree terrains, making their radius = atmosphereAlititude + surface radius for atmo scattering stuff
    }
    
    // Update is called once per frame
    void Update () {
    
        // when camera is far enough away, just hide the planet all together
    }
}
