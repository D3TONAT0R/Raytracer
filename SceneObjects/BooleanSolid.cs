using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

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
			foreach (var s in solids)
			{
				s.Initialize(parentScene);
				s.parent = this;
			}
		}

		public override AABB ComputeLocalShapeBounds()
		{
			foreach(var s in solids)
			{
				s.ComputeLocalShapeBounds();
			}

			if(operation == BooleanOperation.Subtract)
			{
				AABB bounds = AABB.NullBounds;
				foreach (var corner in solids[0].LocalShapeBounds.GetCorners())
				{
					bounds = bounds.Join(WorldToLocalPoint(solids[0].LocalToWorldPoint(corner)));
				}
				return bounds;
			} 
			else if(operation == BooleanOperation.Add)
			{
				var bounds = AABB.NullBounds;
				foreach(var s in solids)
				{
					bounds = bounds.JoinTransformed(this, s.LocalShapeBounds, s);
				}
				return bounds;
			}
			else if(operation == BooleanOperation.Intersect)
			{
				if(solids.Length == 0) return AABB.NullBounds;
				var bounds = AABB.Infinity;
				foreach(var s in solids)
				{
					bounds = bounds.TrimTransformed(this, s.LocalShapeBounds, s);
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

		public override Vector3 GetLocalNormalAt(Vector3 localPos)
		{
			var closest = GetClosestSurfaceShape(localPos);
			Vector3 normal = closest?.GetLocalNormalAt(localPos) ?? Vector3.Zero;
			if(operation == BooleanOperation.Subtract && closest != solids[0]) {
				normal *= -1;
			}
			return normal;
		}

		public override bool Intersects(Vector3 localPos) {
			if(operation == BooleanOperation.Add) {
				bool b = false;
				for(int i = 0; i < solids.Length; i++)
				{
					var shapeLocalPos = solids[i].WorldToLocalPoint(LocalToWorldPoint(localPos));
					if (solids[i].Intersects(shapeLocalPos)) return true;
				}
				return false;
			} else if(operation == BooleanOperation.Intersect) {
				bool b = true;
				for(int i = 0; i < solids.Length; i++) {
					var shapeLocalPos = solids[i].WorldToLocalPoint(LocalToWorldPoint(localPos));
					b &= solids[i].Intersects(shapeLocalPos);
				}
				return b;
			} else if(operation == BooleanOperation.Subtract) {
				if(!solids[0].Intersects(solids[0].WorldToLocalPoint(LocalToWorldPoint(localPos)))) return false;
				for(int i = 1; i < solids.Length; i++) {
					var shapeLocalPos = solids[i].WorldToLocalPoint(LocalToWorldPoint(localPos));
					if(solids[i].Intersects(shapeLocalPos)) {
						return false;
					}
				}
				return true;
			} else {
				return true;
			}
		}

		public override float GetSurfaceProximity(Vector3 localPos) {
			float prox = 999;
			for(int i = 0; i < solids.Length; i++) {
				//if(RaytracedRenderer.scene.shapeAABBs[solids[i]][1].IsInside(localPos)) prox = Math.Min(prox, solids[i].GetSurfaceProximity(localPos));
				if(solids[i].ExpandedLocalShapeBounds.IsInside(localPos)) prox = Math.Min(prox, solids[i].GetSurfaceProximity(localPos));
			}
			return prox;
		}

		private Shape GetClosestSurfaceShape(Vector3 localPos) {
			Shape shape = null;
			float closest = float.MaxValue;
			var worldPos = LocalToWorldPoint(localPos);
			for(int i = 0; i < solids.Length; i++) {
				var shapeLocalPos = solids[i].WorldToLocalPoint(worldPos);
				if(solids[i].ExpandedLocalShapeBounds.IsInside(shapeLocalPos)) {
					var prox = solids[i].GetSurfaceProximity(shapeLocalPos);
					if(prox < closest) {
						shape = solids[i];
						closest = prox;
					}
				}
			}
			return shape;
		}

		public override Color GetColorAt(Vector3 localPos, Ray ray, bool invertNormals = false)
		{
			var shape = GetClosestSurfaceShape(localPos);
			var localShapePos = shape.WorldToLocalPoint(LocalToWorldPoint(localPos));
			if(operation == BooleanOperation.Subtract && shape != solids[0])
			{
				invertNormals = !invertNormals;
			}
			return shape.GetColorAt(localShapePos, ray, invertNormals);
		}

		public override Material GetMaterial(Vector3 localPos) {
			if(material != null) {
				return material;
			} else {
				return GetClosestSurfaceShape(localPos)?.GetMaterial(localPos);
			}
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 localNormal)
		{
			var shape = GetClosestSurfaceShape(localPos);
			return shape.GetUV(localPos, localNormal);
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
