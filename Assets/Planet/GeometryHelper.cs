using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Planet
{
    public static class GeometryHelper
    {
        public static void Triangle(float sizeInUnits, out Vector3[] verts, out int[] indices, out Vector2[] uvs)
        {
            var halfUnits = sizeInUnits*0.5f;
            verts = new[] {
                new Vector3(-halfUnits, 0, -halfUnits), 
                new Vector3(halfUnits, 0, -halfUnits), 
                new Vector3(halfUnits, 0, halfUnits), 
            };

            indices = new[] {
                0, 1, 2
            };

            uvs = new[] {
                Vector2.zero,
                new Vector2(1, 0),
                Vector2.one,
                new Vector2(0, 1), 
            };
        
        }

        public static void Patch(float sizeInUnits, out Vector3[] patchVerts, out int[] patchIndices)
        {
            const float resolution = 33;

            var gridSize = sizeInUnits/resolution;
            var halfUnits = sizeInUnits*0.5f;

            var vertices = new List<Vector3>();
            var indexList = new List<int>();

            for (var x = -halfUnits; x < halfUnits; x += gridSize)
            {
                for (var y = -halfUnits; y < halfUnits; y += gridSize)
                {
                    var vertCount = vertices.Count;
                    indexList.Add(3 + vertCount);
                    indexList.Add(1 + vertCount);
                    indexList.Add(0 + vertCount);

                    indexList.Add(3 + vertCount);
                    indexList.Add(2 + vertCount);
                    indexList.Add(1 + vertCount);

                    vertices.Add(new Vector3(x, 0, y));
                    vertices.Add(new Vector3(x + gridSize, 0, y));

                    vertices.Add(new Vector3(x + gridSize, 0, y + gridSize));
                    vertices.Add(new Vector3(x, 0, y + gridSize));
                }
            }

            patchVerts = vertices.ToArray();
            patchIndices = indexList.ToArray();
        }

        public static void Plane(float sizeInUnits, out Vector3[] verts, out int[] indices)//, out Vector2[] uvs)
        {
            var halfUnits = sizeInUnits*0.5f;
            verts = new[] {
                new Vector3(-halfUnits, 0, -halfUnits), 
                new Vector3(halfUnits, 0, -halfUnits), 

                new Vector3(halfUnits, 0, halfUnits), 
                new Vector3(-halfUnits, 0, halfUnits), 
            };

            indices = new[] {
                3, 1, 0,
                3, 2, 1
            };
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> functor)
        {
            if(functor == null)
                throw new ArgumentNullException("functor");

            var count = 0;
            foreach (var item in source)
                functor(item, count++);
            
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> functor)
        {
            if(functor == null)
                throw new ArgumentNullException("functor");

            foreach (var item in source)
                functor(item);
            
        }

    }
}