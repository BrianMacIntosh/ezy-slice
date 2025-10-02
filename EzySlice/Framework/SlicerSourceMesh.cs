using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EzySlice.Slicer;

namespace EzySlice
{
	/**
	 * Caches mesh data to eliminate allocations from retrieving it.
	 */
	public class SlicerSourceMesh
	{
		public readonly Mesh mesh;

		public readonly Vector3[] vertices;
		public readonly Vector2[] uv;
		public readonly Vector3[] normals;
		public readonly Vector4[] tangents;
		public readonly int[][] submeshTriangles;

		// Recycled buffers used for Slice. NOT thread-safe.

		internal readonly SlicedSubmesh[] slices;
		internal readonly List<Vector3> crossHull;
		internal readonly IntersectionResult intersectionResult;

		public SlicerSourceMesh(Mesh mesh)
		{
			this.mesh = mesh;

			vertices = mesh.vertices;
			uv = mesh.uv;
			normals = mesh.normals;
			tangents = mesh.tangents;

			int submeshCount = mesh.subMeshCount;

			submeshTriangles = new int[submeshCount][];
			slices = new SlicedSubmesh[submeshCount];
			for (int i = 0; i < submeshCount; i++)
			{
				slices[i] = new SlicedSubmesh();
				submeshTriangles[i] = mesh.GetTriangles(i);
			}

			crossHull = new List<Vector3>();
			intersectionResult = new IntersectionResult();
		}

		public void ResetBuffers()
		{
			foreach (SlicedSubmesh submesh in slices)
			{
				submesh.upperHull.Clear();
				submesh.lowerHull.Clear();
			}
			crossHull.Clear();
			intersectionResult.Clear();
		}
	}

	/**
	 * Caches SlicerSourceMesh objects for shared meshes.
	 * You can use this for simple, easy caching, or create your own SlicerSourceMesh objects instead.
	 */
	public static class SlicerMeshCache
	{
		private static Dictionary<Mesh, SlicerSourceMesh> cache = new Dictionary<Mesh, SlicerSourceMesh>();

		public static SlicerSourceMesh Get(Mesh mesh)
		{
			//TODO: detection for cache running away (e.g. not using sharedMesh)
			if (cache.TryGetValue(mesh, out SlicerSourceMesh cachedMesh))
			{
				return cachedMesh;
			}
			else
			{
				SlicerSourceMesh newCache = new SlicerSourceMesh(mesh);
				cache.Add(mesh, newCache);
				return newCache;
			}
		}
	}
}
