using System;
using UnityEngine;

[Serializable]
public class Planet : MonoBehaviour
{
    private GameObject[] _sides;

    [SerializeField]
    [Tooltip("Sea level altitude from the center of the planet in meters")]
    public float SurfaceRadius = 128.0f;

    [SerializeField]
    [Tooltip("The end of the atmosphere in meters. 0 =< sea level")]
    public float AtmosphereAltitude = 128.0f;


    // Use this for initialization
    void Start()
    {
        _sides = new GameObject[] {
        };

        // if the atmo is at/below zero then we just don't render an atmo at all
        if (AtmosphereAltitude <= 0)
        {
            Debug.Log("Not rendering any atmosphere");
        }

        // create 6 quadtree terrains, 1 for each side of the cube
        // tell them to be at a radius = surface radius
        // make 6 more quad tree terrains, making their radius = atmosphereAlititude + surface radius for atmo scattering stuff
    }

    // Update is called once per frame
    void Update()
    {

        // when camera is far enough away, just hide the planet all together
    }
}
