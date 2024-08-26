using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("CUBOID")]
	public class Cuboid : SolidShape {

		static readonly Vector3[] localNormals = new Vector3[] {
			-Vector3.UnitY,
			Vector3.UnitY,
			-Vector3.UnitX,
			Vector3.UnitX,
			-Vector3.UnitZ,
			Vector3.UnitZ
		};

		[DataIdentifier("SIZE", 0.1f)]
		public Vector3 size;

		public Cuboid() : base(null) { }

		public Cuboid(string name, Vector3 position, Vector3 size) : base(name) {
			localPosition = position;
			this.size = size;
		}

		public override void SetupForRendering() {
			ShapeAABB = new AABB(Vector3.Zero, size);
		}

		public override bool Intersects(Vector3 pos) {
			pos = WorldToLocalPoint(pos);
			return
				pos.X > 0 && pos.X < size.X &&
				pos.Y > 0 && pos.Y < size.Y &&
				pos.Z > 0 && pos.Z < size.Z;
			//return ShapeAABB.IsInside(pos);
		}

		public override Vector3 GetLocalNormalAt(Vector3 pos)
		{
			CalculateNearestFace(pos, out int face, out _);
			var normal = localNormals[face];
			//return Vector3.TransformNormal(normal, WorldToLocalMatrix);
			return normal;
		}

		public override float GetSurfaceProximity(Vector3 worldPos) {
			CalculateNearestFace(worldPos, out _, out float prox);
			return prox;
		}

		protected virtual void CalculateNearestFace(Vector3 pos, out int nearestFace, out float proximity) {
			pos = WorldToLocalPoint(pos);
			//0 = bottom
			//1 = top
			//2 = left
			//3 = right
			//4 = back
			//5 = front
			float[] dst = new float[6];
			dst[0] = Math.Abs(pos.Y - ShapeAABB.lower.Y);
			dst[1] = Math.Abs(pos.Y - ShapeAABB.upper.Y);
			dst[2] = Math.Abs(pos.X - ShapeAABB.lower.X);
			dst[3] = Math.Abs(pos.X - ShapeAABB.upper.X);
			dst[4] = Math.Abs(pos.Z - ShapeAABB.lower.Z);
			dst[5] = Math.Abs(pos.Z - ShapeAABB.upper.Z);
			nearestFace = -1;
			proximity = 999;
			for(int i = 0; i < 6; i++) {
				if(dst[i] < proximity) {
					nearestFace = i;
					proximity = dst[i];
				}
			}
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 normal)
		{
			var normPos = localPos / size;
			if(normal.Y > 0.9f)
			{
				//Top
				return new Vector2(normPos.X, normPos.Z);
			}
			else if(normal.Y < -0.9f)
			{
				//Bottom
				return new Vector2(-normPos.X, normPos.Z);
			}
			else if(normal.X > 0.9f)
			{
				//Right
				return new Vector2(normPos.Z, normPos.Y);
			}
			else if(normal.X < -0.9f)
			{
				//Left
				return new Vector2(-normPos.Z, normPos.Y);
			}
			else if(normal.Z < -0.9f)
			{
				//Front
				return new Vector2(normPos.X, normPos.Y);
			}
			else if(normal.Z > 0.9f)
			{
				//Back
				return new Vector2(-normPos.X, normPos.Y);
			}
			else return Vector2.Zero;
		}
	}
}
