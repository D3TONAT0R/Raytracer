using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public abstract class Shape : SceneObject {

		[DataIdentifier("MATERIAL")]
		public Material material;

		public Shape(string name) : base(name) {
		}

		private AABB shapeAABB;

		public AABB ShapeAABB {
			get {
				return shapeAABB;
			}
			protected set {
				shapeAABB = value;
				ExpandedAABB = value.Expand(RaytracerEngine.CurrentRenderSettings.rayMarchDistanceInVoid);
				//ExpandedAABB = value.Expand(0.001f);
			}
		}

		public AABB ExpandedAABB {
			get;
			protected set;
		}

		public override bool CanContainShapes => true;

		public abstract bool Intersects(Vector3 pos);

		public abstract Color GetColorAt(Vector3 pos, Ray ray);

		public abstract Vector3 GetNormalAt(Vector3 pos);

		public virtual Material GetMaterial(Vector3 pos) {
			return material ?? OverrideMaterial;
		}

		public abstract float GetSurfaceProximity(Vector3 worldPos);

		public virtual Shape[] GetSubShapes() {
			return new Shape[] { this };
		}

		public override IEnumerable<Shape> GetIntersectingShapes(Ray ray)
		{
			if(ShapeAABB.Intersects(ray)) yield return this;
		}

		public override AABB GetTotalShapeAABB()
		{
			return ShapeAABB;
		}

		public abstract Vector2 GetUV(Vector3 localPos, Vector3 normal);
	}
}
