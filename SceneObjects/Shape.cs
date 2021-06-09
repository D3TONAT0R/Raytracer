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
				ExpandedAABB = value.Expand(RaytracedRenderer.CurrentSettings.rayMarchDistanceInVoid);
			}
		}

		public AABB ExpandedAABB {
			get;
			protected set;
		}

		public abstract bool Intersects(Vector3 pos);

		public abstract Color GetColorAt(Vector3 pos, Ray ray);

		public abstract Vector3 GetNormalAt(Vector3 pos, Ray ray);

		public virtual Material GetMaterial(Vector3 pos) {
			return material;
		}

		/*public virtual void SetupAABBs(Dictionary<Shape, AABB> expandedAABBs, float expansionAmount) {
			shapeAABB = SetupAABB();
			if(!expandedAABBs.ContainsKey(this)) {
				expandedAABBs.Add(this, shapeAABB.Offset(HierarchyPositionOffset).Expand(expansionAmount));
			}
		}*/

		public abstract void SetupAABBs();

		public virtual void OnBeginRender() {
			
		}

		//public abstract AABB SetupAABB();

		public abstract float GetSurfaceProximity(Vector3 worldPos);

		public virtual Shape[] GetSubShapes() {
			return new Shape[] { this };
		}
	}
}
