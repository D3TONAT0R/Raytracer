using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	public class Volume : SceneObject
	{
		public enum VolumeType
		{
			Box,
			Sphere
		}

		[DataIdentifier("TYPE")]
		public VolumeType volumeType = VolumeType.Box;
		[DataIdentifier("SIZE")]
		public Vector3 size = Vector3.One;
		[DataIdentifier("DENSITY", 0.01f)]
		public float density = 0.1f;
		[DataIdentifier("COLOR")]
		public Color color = Color.White;

		public override void SetupForRendering()
		{
			
		}

		public override AABB ComputeLocalShapeBounds()
		{
			return new AABB(-size * 0.5f, size * 0.5f);
		}

		public float GetDensity(Vector3 localPos)
		{
			if(volumeType == VolumeType.Box)
			{
				if(Math.Abs(localPos.X) <= size.X * 0.5f && Math.Abs(localPos.Y) <= size.Y * 0.5f && Math.Abs(localPos.Z) <= size.Z * 0.5f)
				{
					return density;
				}
			}
			else
			{
				var normPos = localPos / size;
				if(normPos.Length() < 1)
				{
					return density;
				}
			}
			return 0;
		}

		public bool IntersectsRay(Ray ray)
		{
			return ray.GetAABBIntersectionPoints(WorldSpaceShapeBounds, Matrix4x4.Identity, Matrix4x4.Identity, out _, out _) > 0;
		}
	}
}
