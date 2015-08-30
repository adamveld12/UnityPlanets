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

    public class MeshDef
    {
        private readonly Vector3[] _vertices;
        private readonly int[] _indices;

        public MeshDef(Vector3[] vertices, int[] indices)
        {
            _vertices = vertices;
            _indices = indices;
        }

        public Mesh ToMesh()
        {
            var mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _indices
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}