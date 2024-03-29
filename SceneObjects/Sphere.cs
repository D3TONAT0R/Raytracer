﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("SPHERE")]
	public class Sphere : SolidShape {

		[DataIdentifier("RADIUS")]
		public float radius;

		public Sphere() : base(null) { }

		public Sphere(string name, Vector3 position, float radius) : base(name) {
			this.localPosition = position;
			this.radius = radius;
		}

		public override void SetupForRendering() {
			ShapeAABB = new AABB(WorldPosition - Vector3.One * radius, WorldPosition + Vector3.One * radius);
		}

		public override bool Intersects(Vector3 pos) {
			return Vector3.Distance(pos, WorldPosition) <= radius;
		}

		public override Vector3 GetNormalAt(Vector3 pos)
		{
			var nrm = Vector3.Normalize(pos - WorldPosition);
			return nrm;
		}

		public override float GetSurfaceProximity(Vector3 worldPos) {
			return Math.Abs(Vector3.Distance(WorldPosition, worldPos) - radius);
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 normal)
		{
			return new Vector2(MathUtils.Dir2DToAngle01(normal.X, normal.Z), normal.Y * 0.5f + 0.5f);
		}
	}
}
