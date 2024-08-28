using System;
using System.Numerics;

namespace Raytracer
{
	[ObjectIdentifier("WIRE_CUBOID")]
	public class WireCuboid : Shape
	{
		[DataIdentifier("COLOR", 0.25f)]
		public Color color = Color.Red;
		[DataIdentifier("THICKNESS", 0.01f)]
		public float thickness = 0.1f;

		private Shape referenceShape;

		public WireCuboid(Shape boundingShape) : base(null)
		{
			referenceShape = boundingShape;
		}

		public override bool Intersects(Vector3 pos)
		{
			pos = WorldToLocalPoint(pos);
			var aabb = referenceShape.LocalShapeBounds.Expand(thickness * 0.5f);
			float edgeDistX = Math.Min(pos.X - aabb.lower.X, aabb.upper.X - pos.X);
			float edgeDistY = Math.Min(pos.Y - aabb.lower.Y, aabb.upper.Y - pos.Y);
			float edgeDistZ = Math.Min(pos.Z - aabb.lower.Z, aabb.upper.Z - pos.Z);
			int i = 0;
			if(edgeDistX <= thickness) i++;
			if(edgeDistY <= thickness) i++;
			if(edgeDistZ <= thickness) i++;
			return i > 1;
		}

		public override void SetupForRendering()
		{
			parent = referenceShape.parent;
			localPosition = referenceShape.localPosition;
			localRotation = referenceShape.localRotation;
			localScale = referenceShape.localScale;
			color = referenceShape.boundsColor;
			//SetupMatrix();
		}

		public override AABB ComputeLocalShapeBounds()
		{
			return referenceShape.LocalShapeBounds;
		}

		public override Color GetColorAt(Vector3 pos, Ray ray)
		{
			return color;
		}

		public override Vector3 GetLocalNormalAt(Vector3 pos)
		{
			return Vector3.Zero;
		}

		public override float GetSurfaceProximity(Vector3 worldPos)
		{
			return 0;
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 normal)
		{
			return Vector2.Zero;
		}
	}
}
