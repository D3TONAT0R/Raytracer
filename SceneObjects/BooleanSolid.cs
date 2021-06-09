using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("BOOLSOLID")]
	public class BooleanSolid : SolidShape {

		public enum BooleanOperation {
			Union,
			Difference,
			Intersection
		}

		[DataIdentifier("SOLIDS")]
		public readonly SolidShape[] solids;
		[DataIdentifier("OPERATION")]
		public BooleanOperation operation;

		//public AABB[] shapeAABBs;
		//private AABB[] expandedAABBs;

		public BooleanSolid() : base(null) { }

		public BooleanSolid(string name, BooleanOperation type, params SolidShape[] shapes) : base(name) {
			operation = type;
			solids = shapes;
		}

		public override void HandleExtraIdentifier(string extra) {
			var c = extra[0];
			if(c == '+') operation = BooleanOperation.Union;
			else if(c == '-') operation = BooleanOperation.Difference;
			else if(c == 'X') operation = BooleanOperation.Intersection;
		}

		protected override void OnInit() {
			foreach(var s in solids) s.Initialize();
		}

		public override void OnBeginRender() {
			/*shapeAABBs = new AABB[solids.Length];
			for(int i = 0; i < solids.Length; i++) {
				shapeAABBs[i] = solids[i].ShapeAABB.Expand(RaytracedRenderer.CurrentSettings.rayMarchDistanceInObject);
			}*/
		}

		/*public override void SetupAABBs(Dictionary<Shape, AABB> expandedAABBs, float expansionAmount) {
			base.SetupAABBs(expandedAABBs, expansionAmount);
			for(int i = 0; i < solids.Length; i++) {
				solids[i].SetupAABBs(expandedAABBs, expansionAmount);
			}
		}*/

		public override void SetupAABBs() {
			//expandedAABBs = new AABB[solids.Length];
			for(int i = 0; i < solids.Length; i++) {
				solids[i].SetupAABBs();
				//expandedAABBs[i] = solids[i].ShapeAABB.Expand(RaytracedRenderer.CurrentSettings.rayMarchDistanceInVoid);
			}

			if(operation == BooleanOperation.Difference) {
				ShapeAABB = solids[0].ShapeAABB;
			} else if(operation == BooleanOperation.Union) {
				ShapeAABB = new AABB();
				foreach(var s in solids) {
					ShapeAABB = ShapeAABB.Join(s.ShapeAABB);
				}
			} else if(operation == BooleanOperation.Intersection) {
				ShapeAABB = AABB.CreateInfinity();
				foreach(var s in solids) {
					ShapeAABB = ShapeAABB.Trim(s.ShapeAABB);
				}
			}
		}

		public override Vector3 GetNormalAt(Vector3 pos, Ray ray) {
			var closest = GetClosestSurfaceShape(pos);
			Vector3 normal = closest.GetNormalAt(pos, ray);
			if(operation == BooleanOperation.Difference && closest != solids[0]) {
				normal *= -1;
			}
			return normal;
		}

		public override bool Intersects(Vector3 pos) {
			if(operation == BooleanOperation.Union) {
				bool b = false;
				for(int i = 0; i < solids.Length; i++) {
					b |= solids[i].Intersects(pos);
				}
				return b;
			} else if(operation == BooleanOperation.Intersection) {
				bool b = true;
				for(int i = 0; i < solids.Length; i++) {
					b &= solids[i].Intersects(pos);
				}
				return b;
			} else if(operation == BooleanOperation.Difference) {
				if(!solids[0].Intersects(pos)) return false;
				for(int i = 1; i < solids.Length; i++) {
					if(solids[i].Intersects(pos)) {
						return false;
					}
				}
				return true;
			} else {
				return true;
			}
		}

		public override float GetSurfaceProximity(Vector3 worldPos) {
			float prox = 999;
			for(int i = 0; i < solids.Length; i++) {
				//if(RaytracedRenderer.scene.shapeAABBs[solids[i]][1].IsInside(worldPos)) prox = Math.Min(prox, solids[i].GetSurfaceProximity(worldPos));
				if(solids[i].ExpandedAABB.IsInside(worldPos)) prox = Math.Min(prox, solids[i].GetSurfaceProximity(worldPos));
			}
			return prox;
		}

		private Shape GetClosestSurfaceShape(Vector3 pos) {
			Shape shape = null;
			float closest = 999;
			for(int i = 0; i < solids.Length; i++) {
				if(solids[i].ShapeAABB.IsInside(pos)) {
					var prox = solids[i].GetSurfaceProximity(pos);
					if(prox < closest) {
						shape = solids[i];
						closest = prox;
					}
				}
			}
			return shape;
		}

		public override Material GetMaterial(Vector3 pos) {
			if(material != null) {
				return material;
			} else {
				return GetClosestSurfaceShape(pos).GetMaterial(pos);
			}
		}

		/*public override void RegisterExpandedAABB(Dictionary<Shape, AABB> expandedAABBs, float expansionAmount) {
			base.RegisterExpandedAABB(expandedAABBs, expansionAmount);
			foreach(var s in solids) {
				s.RegisterExpandedAABB(expandedAABBs, expansionAmount);
			}
		}*/

		/*public override Shape[] GetSubShapes() {
			List<Shape> list = new List<Shape>();
			list.AddRange(base.GetSubShapes());
			foreach(var s in solids) {
				list.AddRange(s.GetSubShapes());
			}
			return list.ToArray();
		}*/
	}
}
