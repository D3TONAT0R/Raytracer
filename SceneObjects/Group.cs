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
		[DataIdentifier("MATERIAL")]
		public Material overrideMaterial;

		public override Material OverrideMaterial => overrideMaterial ?? parent?.OverrideMaterial;

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

		/*
		public T[] GetObjectsOfType<T>() where T : SceneObject {
			List<T> l = new List<T>();
			foreach(var c in children) {
				if(c is Group g) {
					l.AddRange(g.GetObjectsOfType<T>());
				}
				else if(c is ObjectInstance i)
				{
					var refObj = i.referencedObject;
					l.AddRange(i.GetObjectsOfType<T>());
				}
				else if(c is T t) {
					l.Add(t);
				}
			}
			return l.ToArray();
		}
		*/

		public override SceneObject Clone()
		{
			var clone = (Group)base.Clone();
			clone.children = new SceneObject[children.Length];
			for(int i = 0; i < children.Length; i++)
			{
				clone.children[i] = children[i].Clone();
				//clone.children[i].Uninitialize();
			}
			return clone;
		}

		public override IEnumerable<T> GetContainedObjectsOfType<T>()
		{
			foreach(var c in children)
			{
				if(c == null) throw new NullReferenceException("Null child object detected.");
				foreach(var o in c.GetContainedObjectsOfType<T>()) yield return o;
			}
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
