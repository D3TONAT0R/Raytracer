using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public abstract class Shape : SceneObject {

		[DataIdentifier("VISFLAGS")]
		public VisibilityFlags visibilityFlags = VisibilityFlags.All;
		[DataIdentifier("MATERIAL")]
		public Material material;

		[DataIdentifier("DISPLAY_BOUNDS")]
		public bool displayBounds = false;
		[DataIdentifier("BOUNDS_COLOR", 0.25f)]
		public Color boundsColor = Color.Red;

		private WireCuboid boundsCube;

		public override bool CanContainShapes => true;

		public Shape(string name) : base(name) 
		{

		}

		public abstract override AABB ComputeLocalShapeBounds();

		public abstract bool Intersects(Vector3 pos);

		public abstract Color GetColorAt(Vector3 pos, Ray ray);

		public abstract Vector3 GetLocalNormalAt(Vector3 pos);

		public virtual Material GetMaterial(Vector3 pos) {
			return material ?? OverrideMaterial;
		}

		public abstract float GetSurfaceProximity(Vector3 worldPos);

		public virtual Shape[] GetSubShapes() {
			return new Shape[] { this };
		}

		public override IEnumerable<Shape> GetIntersectingShapes(Ray ray)
		{
			var tRay = ray.Transform(WorldToLocalMatrix);
			if(LocalShapeBounds.Intersects(tRay)) yield return this;
		}

		public abstract Vector2 GetUV(Vector3 localPos, Vector3 normal);

		public WireCuboid GetBoundsCube()
		{
			if(boundsCube == null)
			{
				boundsCube = new WireCuboid(this);
			}
			return boundsCube;
		}

		public sealed override IEnumerable<Shape> GetIntersectingShapes(Ray ray, VisibilityFlags flags)
		{
			foreach(var s in GetIntersectingShapes(ray))
			{
				if(s.visibilityFlags.HasFlag(flags))
				{
					yield return s;
				}
				if(s.displayBounds)
				{
					var bounds = s.GetBoundsCube();
					bounds.SetupForRendering();
					if(bounds.visibilityFlags.HasFlag(flags))
					{
						yield return bounds;
					}
				}
			}
		}
	}
}
