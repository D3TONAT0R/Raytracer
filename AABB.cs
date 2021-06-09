using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class AABB {

		public static AABB CreateInfinity() => new AABB(-Vector3.One * float.MaxValue, Vector3.One * float.MaxValue);

		public Vector3 lower;
		public Vector3 upper;
		public Vector3 Size => upper - lower;

		public Vector3 Center => (upper + lower) / 2f;

		public AABB() {

		}

		public AABB(Vector3 a, Vector3 b) {
			lower.X = Math.Min(a.X, b.X);
			lower.Y = Math.Min(a.Y, b.Y);
			lower.Z = Math.Min(a.Z, b.Z);
			upper.X = Math.Max(a.X, b.X);
			upper.Y = Math.Max(a.Y, b.Y);
			upper.Z = Math.Max(a.Z, b.Z);
		}

		public bool IsInside(Vector3 pt) {
			return pt.X.Range(lower.X, upper.X) && pt.Y.Range(lower.Y, upper.Y) && pt.Z.Range(lower.Z, upper.Z);
		}

		public bool Intersects(Ray r) {
			/*Vector3 t1 = lower - ray.position * ray.inverse;
			Vector3 t2 = upper - ray.position * ray.inverse;

			var min = Highest(Min(t1, t2));
			var max = Lowest(Max(t1, t2));

			return max >= min;*/

			float tx1 = (lower.X - r.position.X) * r.Inverse.X;
			float tx2 = (upper.X - r.position.X) * r.Inverse.X;

			float tmin = Math.Min(tx1, tx2);
			float tmax = Math.Max(tx1, tx2);

			float ty1 = (lower.Y - r.position.Y) * r.Inverse.Y;
			float ty2 = (upper.Y - r.position.Y) * r.Inverse.Y;

			tmin = Math.Max(tmin, Math.Min(ty1, ty2));
			tmax = Math.Min(tmax, Math.Max(ty1, ty2));

			float tz1 = (lower.Z - r.position.Z) * r.Inverse.Z;
			float tz2 = (upper.Z - r.position.Z) * r.Inverse.Z;

			tmin = Math.Max(tmin, Math.Min(tz1, tz2));
			tmax = Math.Min(tmax, Math.Max(tz1, tz2));

			return tmax >= tmin;

			/*
			double tx1 = (b.min.x - r.x0.x) * r.n_inv.x;
			double tx2 = (b.max.x - r.x0.x) * r.n_inv.x;

			double tmin = min(tx1, tx2);
			double tmax = max(tx1, tx2);

			double ty1 = (b.min.y - r.x0.y) * r.n_inv.y;
			double ty2 = (b.max.y - r.x0.y) * r.n_inv.y;

			tmin = max(tmin, min(ty1, ty2));
			tmax = min(tmax, max(ty1, ty2));

			return tmax >= tmin;
			*/
		}

		public AABB Expand(float v) {
			return new AABB {
				lower = lower - Vector3.One * v,
				upper = upper + Vector3.One * v
			};
		}

		public AABB Join(AABB other) {
			var aabb = new AABB();
			aabb.lower.X = Math.Min(lower.X, other.lower.X);
			aabb.lower.Y = Math.Min(lower.Y, other.lower.Y);
			aabb.lower.Z = Math.Min(lower.Z, other.lower.Z);
			aabb.upper.X = Math.Max(upper.X, other.upper.X);
			aabb.upper.Y = Math.Max(upper.Y, other.upper.Y);
			aabb.upper.Z = Math.Max(upper.Z, other.upper.Z);
			return aabb;
		}

		public AABB Trim(AABB other) {
			var aabb = new AABB();
			aabb.lower.X = Math.Max(lower.X, other.lower.X);
			aabb.lower.Y = Math.Max(lower.Y, other.lower.Y);
			aabb.lower.Z = Math.Max(lower.Z, other.lower.Z);
			aabb.upper.X = Math.Min(upper.X, other.upper.X);
			aabb.upper.Y = Math.Min(upper.Y, other.upper.Y);
			aabb.upper.Z = Math.Min(upper.Z, other.upper.Z);
			return aabb;
		}

		public AABB ShrinkRelative(Vector3 lowerShrink, Vector3 upperShrink) {
			return new AABB() {
				lower = lower + lowerShrink * Size,
				upper = upper - upperShrink * Size,
			};
		}

		public AABB Offset(Vector3 offset) {
			if(offset == Vector3.Zero) {
				return this;
			} else {
				return new AABB(lower + offset, upper + offset);
			}
		}

		private Vector3 Min(Vector3 a, Vector3 b) {
			return new Vector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
		}

		private Vector3 Max(Vector3 a, Vector3 b) {
			return new Vector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
		}

		private float Lowest(Vector3 v) {
			return Math.Min(v.X, Math.Min(v.Y, v.Z));
		}

		private float Highest(Vector3 v) {
			return Math.Max(v.X, Math.Max(v.Y, v.Z));
		}
	}
}
