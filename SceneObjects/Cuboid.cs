using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

		[DataIdentifier("SIZE")]
		public Vector3 size;

		public Cuboid() : base(null) { }

		public Cuboid(string name, Vector3 position, Vector3 size) : base(name) {
			localPosition = position;
			this.size = size;
		}

		public override void SetupAABBs() {
			ShapeAABB = new AABB(WorldPosition, WorldPosition + size);
		}

		public override bool Intersects(Vector3 pos) {
			return ShapeAABB.IsInside(pos);
		}

		public override Vector3 GetNormalAt(Vector3 pos, Ray ray) {
			CalculateNearestFace(pos, out int face, out _);
			return localNormals[face];
		}

		public override float GetSurfaceProximity(Vector3 worldPos) {
			CalculateNearestFace(worldPos, out _, out float prox);
			return prox;
		}

		protected virtual void CalculateNearestFace(Vector3 pos, out int nearestFace, out float proximity) {
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
	}
}
