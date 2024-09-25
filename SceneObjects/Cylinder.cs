using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("CYLINDER")]
	public class Cylinder : SolidShape {

		[DataIdentifier("AXIS")]
		public Axis axis = Axis.Y;
		[DataIdentifier("RADIUS", 0.1f)]
		public float radius = 1;
		[DataIdentifier("LENGTH", 0.1f)]
		public float length = 1;

		public Cylinder() : base(null) { }

		public Cylinder(string name, Vector3 pos, float r, float l, Axis direction = Axis.Y) : base(name) {
			localPosition = pos;
			radius = r;
			length = l;
			axis = direction;
		}

		public override void SetupForRendering() {
			
		}

		//TODO: rotated y cylinders are cut off when rotated around the x axis
		public override AABB ComputeLocalShapeBounds()
		{
			Vector3 lower = Vector3.Zero;
			Vector3 upper = Vector3.Zero;
			if(axis == Axis.Y)
			{
				lower.X -= radius;
				lower.Z -= radius;
				upper.X += radius;
				upper.Z += radius;
				upper.Y += length;
			}
			else if(axis == Axis.X)
			{
				lower.Z -= radius;
				lower.Y -= radius;
				upper.Z += radius;
				upper.Y += radius;
				upper.X += length;
			}
			else if(axis == Axis.Z)
			{
				lower.X -= radius;
				lower.Y -= radius;
				upper.X += radius;
				upper.Y += radius;
				upper.Z += length;
			}
			return new AABB(lower, upper);
		}

		public override float GetSurfaceProximity(Vector3 localPos) {
			CalculateClosestFace(localPos, out _, out float prox);
			return prox;
		}

		public override bool Intersects(Vector3 localPos) {
			if(axis == Axis.Y) {
				return localPos.Y.Range(LocalShapeBounds.lower.Y, LocalShapeBounds.upper.Y) && localPos.XZ().Length() < radius;
			} else if(axis == Axis.X) {
				return localPos.X.Range(LocalShapeBounds.lower.X, LocalShapeBounds.upper.X) && localPos.ZY().Length() < radius;
			} else if(axis == Axis.Z) {
				return localPos.Z.Range(LocalShapeBounds.lower.Z, LocalShapeBounds.upper.Z) && localPos.XY().Length() < radius;
			}
			return true;
		}

		public override Vector3 GetLocalNormalAt(Vector3 localPos)
		{
			CalculateClosestFace(localPos, out int face, out _);
			if(axis == Axis.Y) {
				if(face == 0) {
					var xz = localPos.XZ();
					return Vector3.Normalize(new Vector3(xz.X, 0, xz.Y));
				} else if(face == -1) {
					return -Vector3.UnitY;
				} else if(face == 1) {
					return Vector3.UnitY;
				}
			} else if(axis == Axis.X) {
				if(face == 0) {
					var zy = localPos.ZY();
					return Vector3.Normalize(new Vector3(0, zy.Y, zy.X));
				} else if(face == -1) {
					return -Vector3.UnitX;
				} else if(face == 1) {
					return Vector3.UnitX;
				}
			} else if(axis == Axis.Z) {
				if(face == 0) {
					var xy = localPos.XY();
					return Vector3.Normalize(new Vector3(xy.X, xy.Y, 0));
				} else if(face == -1) {
					return -Vector3.UnitZ;
				} else if(face == 1) {
					return Vector3.UnitZ;
				}
			}
			return Vector3.UnitY;
		}

		private void CalculateClosestFace(Vector3 localPos, out int face, out float prox) {
			var center = Vector3.Zero;
			face = 0;
			prox = 0;
			float proxL = 0;
			float proxU = 0;
			if(axis == Axis.Y) {
				prox = Math.Abs(Vector2.Distance(localPos.XZ(), center.XZ()) - radius);
				proxL = Math.Abs(localPos.Y - LocalShapeBounds.lower.Y);
				proxU = Math.Abs(localPos.Y - LocalShapeBounds.upper.Y);
			} else if(axis == Axis.X) {
				prox = Math.Abs(Vector2.Distance(localPos.ZY(), center.ZY()) - radius);
				proxL = Math.Abs(localPos.X - LocalShapeBounds.lower.X);
				proxU = Math.Abs(localPos.X - LocalShapeBounds.upper.X);
			} else if(axis == Axis.Z) {
				prox = Math.Abs(Vector2.Distance(localPos.XY(), center.XY()) - radius);
				proxL = Math.Abs(localPos.Z - LocalShapeBounds.lower.Z);
				proxU = Math.Abs(localPos.Z - LocalShapeBounds.upper.Z);
			}
			if(proxL < prox) {
				face = -1;
				prox = proxL;
			}
			if(proxU < prox) {
				face = 1;
				prox = proxU;
			}
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 localNormal)
		{
			if(axis == Axis.X)
			{
				if(localNormal.X > 0.9f)
				{
					//Top face uv
					return new Vector2(localPos.Z * 0.5f + 0.5f, localPos.Y * 0.5f + 0.5f);
				}
				else if(localNormal.X < -0.9f)
				{
					//Bottom face uv
					return new Vector2(localPos.Z * 0.5f + 0.5f, localPos.Y * 0.5f + 0.5f);
				}
				else
				{
					//Side uv
					Vector3 normPos = new Vector3(localPos.X / length, localPos.Y / radius, localPos.Z / radius);
					return new Vector2(MathUtils.Dir2DToAngle01(normPos.Y, normPos.Z), normPos.X);
				}
			}
			else if(axis == Axis.Y)
			{
				if(localNormal.Y > 0.9f)
				{
					//Top face uv
					return new Vector2(localPos.X * 0.5f + 0.5f, localPos.Z * 0.5f + 0.5f);
				}
				else if(localNormal.Y < -0.9f)
				{
					//Bottom face uv
					return new Vector2(localPos.X * 0.5f + 0.5f, localPos.Z * 0.5f + 0.5f);
				}
				else
				{
					//Side uv
					Vector3 normPos = new Vector3(localPos.X / radius, localPos.Y / length, localPos.Z / radius);
					return new Vector2(MathUtils.Dir2DToAngle01(normPos.X, normPos.Z), normPos.Y);
				}
			}
			else if(axis == Axis.Z)
			{
				if(localNormal.Z > 0.9f)
				{
					//Top face uv
					return new Vector2(localPos.X * 0.5f + 0.5f, localPos.Y * 0.5f + 0.5f);
				}
				else if(localNormal.Z < -0.9f)
				{
					//Bottom face uv
					return new Vector2(localPos.X * 0.5f + 0.5f, localPos.Y * 0.5f + 0.5f);
				}
				else
				{
					//Side uv
					Vector3 normPos = new Vector3(localPos.X / radius, localPos.Y / radius, localPos.Z / length);
					return new Vector2(MathUtils.Dir2DToAngle01(normPos.X, normPos.Y), normPos.Z);
				}
			}
			else
			{
				return Vector2.Zero;
			}
		}
	}
}
