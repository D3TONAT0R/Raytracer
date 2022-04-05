using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("GROUP")]
	public class Group : SceneObject {

		[DataIdentifier("CHILDREN")]
		public SceneObject[] children = new SceneObject[0];

		public Group() : base(null) { }

		public Group(string name, Vector3 pivot, params SceneObject[] content) : base(name) {
			localPosition = pivot;
			children = content;
		}

		protected override void OnInit() {
			foreach(var c in children) {
				c.Initialize();
				c.parent = this;
			}
		}

		public Shape[] GetShapes() {
			return GetObjectsOfType<Shape>();
		}

		public Light[] GetLights() {
			return GetObjectsOfType<Light>();
		}

		public T[] GetObjectsOfType<T>() where T : SceneObject {
			List<T> l = new List<T>();
			foreach(var c in children) {
				if(c is Group) {
					l.AddRange(GetObjectsOfType<T>());
				} else if(c is T t) {
					l.Add(t);
				}
			}
			return l.ToArray();
		}

		/*public List<AABB> shapeAABBs;

		public override void OnBeginRender() {
			shapeAABBs = new List<AABB>();
			for(int i = 0; i < children.Length; i++) {
				shapeAABBs[i] = solids[i].ShapeAABB.Expand(RaytracedRenderer.CurrentSettings.rayMarchDistanceInObject);
			}
		}

		public override void RegisterExpandedAABB(Dictionary<Shape, AABB> expandedAABBs, float expansionAmount) {
			base.RegisterExpandedAABB(expandedAABBs, expansionAmount);
			for(int i = 0; i < solids.Length; i++) {
				solids[i].RegisterExpandedAABB(expandedAABBs, expansionAmount);
			}
		}*/
	}
}
