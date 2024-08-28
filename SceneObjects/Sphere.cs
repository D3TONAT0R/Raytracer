using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("SPHERE")]
	public class Sphere : SolidShape {

		[DataIdentifier("RADIUS", 0.1f)]
		public float radius;

		public Sphere() : base(null) { }

		public Sphere(string name, Vector3 position, float radius) : base(name) {
			this.localPosition = position;
			this.radius = radius;
		}

		public override void SetupForRendering() {
			
		}

		public override AABB ComputeLocalShapeBounds()
		{
			return new AABB(-Vector3.One * radius, Vector3.One * radius);
		}

		public override bool Intersects(Vector3 pos) {
			pos = WorldToLocalPoint(pos);
			return pos.Length() <= radius;
		}

		public override Vector3 GetLocalNormalAt(Vector3 worldPos)
		{
			worldPos = WorldToLocalPoint(worldPos);
			var nrm = Vector3.Normalize(worldPos);
			return nrm;
		}

		public override float GetSurfaceProximity(Vector3 worldPos)
		{
			var localPos = WorldToLocalPoint(worldPos);
			return Math.Abs(localPos.Length() - radius);
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 normal)
		{
			return new Vector2(MathUtils.Dir2DToAngle01(normal.X, normal.Z), normal.Y * 0.5f + 0.5f);
		}
	}
}
