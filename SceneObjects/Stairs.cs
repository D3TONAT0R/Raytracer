using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	[ObjectIdentifier("STAIRS")]
	public class Stairs : SceneObject
	{

		[DataIdentifier("SIZE")]
		public Vector3 size;
		[DataIdentifier("DIRECTION")]
		public SlopeDirection stairsDirection;
		[DataIdentifier("STEPS")]
		public int stepCount;
		[DataIdentifier("MATERIAL")]
		public Material material;

		public override Material OverrideMaterial => material ?? parent?.OverrideMaterial;

		private Shape[] subShapes;

		public override bool CanContainShapes => true;

		protected override void OnInit()
		{
			subShapes = new Shape[stepCount - 1];
			for (int i = 0; i < stepCount - 1; i++)
			{
				Cuboid step;
				float xn = 0f;
				float xp = 1f;
				float zn = 0f;
				float zp = 1f;
				float stepOffsetY = (i / (float)stepCount) * size.Y;
				float stepHeight = size.Y / stepCount;
				switch (stairsDirection)
				{
					case SlopeDirection.XNeg:
						xp = 1f - i / (stepCount - 1f);
						break;
					case SlopeDirection.XPos:
						xn = i / (stepCount - 1f);
						break;
					case SlopeDirection.ZNeg:
						zp = 1f - i / (stepCount - 1f);
						break;
					case SlopeDirection.ZPos:
						zn = i / (stepCount - 1f);
						break;
				}
				step = new Cuboid("step_" + i, new Vector3(size.X * xn, stepOffsetY, size.Z * zn), new Vector3(size.X * (xp - xn), stepHeight, size.Z * (zp - zn)));
				step.parent = this;
				subShapes[i] = step;
			}
		}

		public override IEnumerable<T> GetContainedObjectsOfType<T>()
		{
			foreach(var s in subShapes)
			{
				foreach (var s1 in s.GetContainedObjectsOfType<T>())
				{
					yield return s1;
				}
			}
		}

		public override void SetupForRendering()
		{
			foreach(var ss in subShapes) ss.SetupForRendering();
		}

		public override AABB GetTotalShapeAABB()
		{
			return new AABB(WorldPosition, WorldPosition + size);
		}

		public override IEnumerable<Shape> GetIntersectingShapes(Ray ray)
		{
			foreach(var s in subShapes)
			{
				foreach(var s1 in s.GetIntersectingShapes(ray)) yield return s1;
			}
		}
	}
}