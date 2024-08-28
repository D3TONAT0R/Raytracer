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

		public override bool Intersects(Vector3 localPos)
		{
			var aabb = referenceShape.LocalShapeBounds.Expand(thickness * 0.5f);
			float edgeDistX = Math.Min(localPos.X - aabb.lower.X, aabb.upper.X - localPos.X);
			float edgeDistY = Math.Min(localPos.Y - aabb.lower.Y, aabb.upper.Y - localPos.Y);
			float edgeDistZ = Math.Min(localPos.Z - aabb.lower.Z, aabb.upper.Z - localPos.Z);
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

		public override Color GetColorAt(Vector3 localPos, Ray ray, bool invertNormals = false)
		{
			return color;
		}

		public override Vector3 GetLocalNormalAt(Vector3 localPos)
		{
			return Vector3.Zero;
		}

		public override float GetSurfaceProximity(Vector3 localPos)
		{
			return 0;
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 localNormal)
		{
			return Vector2.Zero;
		}
	}
}
