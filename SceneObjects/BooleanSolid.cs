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
			Add,
			Subtract,
			Intersect
		}

		[DataIdentifier("TYPE")]
		public BooleanOperation operation;
		[DataIdentifier("SOLIDS")]
		public readonly SolidShape[] solids;

		//public AABB[] shapeAABBs;
		//private AABB[] expandedAABBs;

		public BooleanSolid() : base(null) { }

		public BooleanSolid(string name, BooleanOperation type, params SolidShape[] shapes) : base(name) {
			operation = type;
			solids = shapes;
		}

		protected override void OnInit(Scene parentScene)
		{
			foreach(var s in solids) s.Initialize(parentScene);
		}

		public override AABB ComputeLocalShapeBounds()
		{
			foreach(var s in solids)
			{
				s.ComputeLocalShapeBounds();
			}

			if(operation == BooleanOperation.Subtract)
			{
				return solids[0].LocalShapeBounds;
			} 
			else if(operation == BooleanOperation.Add)
			{
				var bounds = AABB.NullBounds;
				foreach(var s in solids)
				{
					bounds = bounds.Join(s.LocalShapeBounds);
				}
				return bounds;
			}
			else if(operation == BooleanOperation.Intersect)
			{
				if(solids.Length == 0) return AABB.NullBounds;
				var bounds = AABB.CreateInfinity();
				foreach(var s in solids)
				{
					bounds = bounds.Trim(s.LocalShapeBounds);
				}
				return bounds;
			}
			return AABB.NullBounds;
		}

		public override void SetupForRendering()
		{
			for(int i = 0; i < solids.Length; i++)
			{
				solids[i].SetupForRendering();
			}
		}

		public override Vector3 GetLocalNormalAt(Vector3 worldPos)
		{
			var closest = GetClosestSurfaceShape(worldPos);
			Vector3 normal = closest?.GetLocalNormalAt(worldPos) ?? Vector3.Zero;
			if(operation == BooleanOperation.Subtract && closest != solids[0]) {
				normal *= -1;
			}
			return normal;
		}

		public override bool Intersects(Vector3 pos) {
			if(operation == BooleanOperation.Add) {
				bool b = false;
				for(int i = 0; i < solids.Length; i++) {
					b |= solids[i].Intersects(pos);
				}
				return b;
			} else if(operation == BooleanOperation.Intersect) {
				bool b = true;
				for(int i = 0; i < solids.Length; i++) {
					b &= solids[i].Intersects(pos);
				}
				return b;
			} else if(operation == BooleanOperation.Subtract) {
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
				if(solids[i].ExpandedLocalShapeBounds.IsInside(worldPos)) prox = Math.Min(prox, solids[i].GetSurfaceProximity(worldPos));
			}
			return prox;
		}

		private Shape GetClosestSurfaceShape(Vector3 pos) {
			Shape shape = null;
			float closest = float.MaxValue;
			for(int i = 0; i < solids.Length; i++) {
				if(solids[i].ExpandedLocalShapeBounds.IsInside(pos)) {
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
				return GetClosestSurfaceShape(pos)?.GetMaterial(pos);
			}
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 normal)
		{
			var wp = LocalToWorldPoint(localPos);
			var shape = GetClosestSurfaceShape(wp);
			return shape.GetUV(WorldToLocalPoint(wp), normal);
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

		public override IEnumerable<T> GetContainedObjectsOfType<T>()
		{
			foreach(var s in solids)
			{
				foreach(var o in s.GetContainedObjectsOfType<T>()) yield return o;
			}
		}
	}
}
