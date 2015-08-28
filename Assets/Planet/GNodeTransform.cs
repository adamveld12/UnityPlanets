using UnityEngine;

namespace Assets.Planet
{
    public class GNodeTransform
    {
        public GNodeTransform(Vector3 location, Quaternion rotation)
        {
            Location = location;
            Rotation = rotation;
        }

        public GNodeTransform()
        {
            Location = Vector3.zero;
            Rotation = Quaternion.identity;
        }

        public Vector3 Location { get; set; }
        public Quaternion Rotation { get; set; }
        public GNodeTransform Parent { get; set; }

        public Vector3 TransformVector => Parent?.TransformVector + Location ?? Location;
        public Quaternion TransformRotation => Quaternion.Euler(Parent?.Rotation.eulerAngles + Rotation.eulerAngles ?? Rotation.eulerAngles);
    }
}