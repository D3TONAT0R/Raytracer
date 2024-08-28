using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("PRISM")]
	public class Prism : Cuboid {

		protected class Rect {

			public float left;
			public float bottom;
			public float right;
			public float top;

			public Rect(float l, float b, float r, float t) {
				left = l;
				bottom = b;
				right = r;
				top = t;
			}

			public bool Contains(float x, float y) {
				return x.Range(left, right) && y.Range(bottom, top);
			}
		}

		protected float[] cuts = new float[4] { 0.5f, 0, 0.5f, 0 };
		[DataIdentifier("AXIS")]
		public Axis axis = Axis.X;

		protected bool HasTopFace => cuts[0] - (1f - cuts[1]) > 0.01f && cuts[2] - (1f - cuts[3]) > 0.01f;

		public Prism() : base() { }

		public Prism(string name, Vector3 position, Vector3 size) : this(name, position, size, Axis.X)
		{

		}

		public Prism(string name, Vector3 position, Vector3 size, Axis axis) : base(name, position, size) {
			this.axis = axis;
		}

		protected override void OnInit(Scene parentScene)
		{
			base.OnInit(parentScene);
			if(axis == Axis.Z)
			{
				cuts = new float[4] { 0.5f, 0, 0.5f, 0 };
			}
			else if(axis == Axis.X)
			{
				cuts = new float[4] { 0, 0.5f, 0, 0.5f };
			}
			else
			{
				cuts = new float[4] { 0.5f, 0.5f, 0.5f, 0.5f };
			}
		}

		public override bool Intersects(Vector3 localPos) {
			if(!base.Intersects(localPos)) return false;
			Vector3 rel = localPos / size;
			return GetIntersectingArea(rel.Y).Contains(rel.X, rel.Z);
		}

		protected virtual Rect GetIntersectingArea(float relY) {
			return new Rect(cuts[0] * relY, cuts[1] * relY, 1f - cuts[2] * relY, 1f - cuts[3] * relY);
		}

		public override Vector3 GetLocalNormalAt(Vector3 localPos)
		{
			CalculateNearestFace(localPos, out int face, out _);
			Vector3 nrm;
			if(face == 0) nrm = -Vector3.UnitY;
			else if(face == 1) nrm = Vector3.UnitY;
			else if(face == 2) nrm = Vector3.Lerp(Vector3.UnitY, -Vector3.UnitX, GetSlope(size.X, cuts[0]));
			else if(face == 3) nrm = Vector3.Lerp(Vector3.UnitY, Vector3.UnitX, GetSlope(size.X, cuts[2]));
			else if(face == 4) nrm = Vector3.Lerp(Vector3.UnitY, -Vector3.UnitZ, GetSlope(size.Z, cuts[1]));
			else if(face == 5) nrm = Vector3.Lerp(Vector3.UnitY, Vector3.UnitZ, GetSlope(size.Z, cuts[3]));
			else nrm = Vector3.UnitY;
			return Vector3.Normalize(nrm);
		}

		private float GetSlope(float h, float cut) {
			if(cut > 0) {
				return (float)(Math.Atan(size.Y / (h * cut)) / Math.PI * 2);
			} else {
				return 1;
			}
		}

		protected override void CalculateNearestFace(Vector3 localPos, out int nearestFace, out float proximity) {
			var intersection = GetIntersectingArea((localPos / size).Y);
			var cut = LocalShapeBounds.ShrinkRelative(new Vector3(intersection.left, 0, intersection.bottom), new Vector3(1 - intersection.right, 0, 1 - intersection.top));
			//0 = bottom
			//1 = top
			//2 = left
			//3 = right
			//4 = back
			//5 = front
			float[] dst = new float[6];
			dst[0] = Math.Abs(localPos.Y - cut.lower.Y);
			dst[1] = Math.Abs(localPos.Y - cut.upper.Y);
			dst[2] = Math.Abs(localPos.X - cut.lower.X);
			dst[3] = Math.Abs(localPos.X - cut.upper.X);
			dst[4] = Math.Abs(localPos.Z - cut.lower.Z);
			dst[5] = Math.Abs(localPos.Z - cut.upper.Z);
			nearestFace = -1;
			proximity = 999;
			for(int i = 0; i < 6; i++) {
				if(dst[i] < proximity) {
					if(i == 1 && !HasTopFace) continue;
					nearestFace = i;
					proximity = dst[i];
				}
			}
		}
	}
}
