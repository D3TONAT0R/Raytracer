using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	[ObjectIdentifier("ARRAY")]
	public class SceneObjectArray : SceneObject, IReferencedObject
	{
		public SceneObject[] arrayInstances;
		[DataIdentifier("OFFSET")]
		public Vector3 offset = new Vector3(0, 0, 1);
		[DataIdentifier("COUNT")]
		public int instanceCount = 1;
		[DataIdentifier("MATERIAL")]
		public Material overrideMaterial;

		private AABB totalAABB;

		public string ReferencedObjectName { get; set; }

		public override Material OverrideMaterial => overrideMaterial ?? parent?.OverrideMaterial;

		public override bool CanContainShapes => true;

		protected override void OnInit()
		{
			var refObj = RaytracerEngine.Scene.GetPrefabOrSceneObject(ReferencedObjectName);
			if(refObj == null)
			{
				throw new NullReferenceException($"The referenced SceneObject '{ReferencedObjectName}' does not exist.");
			}
			arrayInstances = new SceneObject[instanceCount];
			for(int i = 0; i < instanceCount; i++)
			{
				var inst = refObj.Clone();
				inst.localPosition += i * offset;
				inst.parent = this;
				inst.Initialize();
				arrayInstances[i] = inst;
			}
		}

		public override IEnumerable<T> GetContainedObjectsOfType<T>()
		{
			foreach(var i in arrayInstances)
			{
				foreach(var o in i.GetContainedObjectsOfType<T>()) yield return o;
			}
		}

		public override IEnumerable<Shape> GetIntersectingShapes(Ray ray)
		{

			foreach(var i in arrayInstances)
			{
				foreach(var s in i.GetIntersectingShapes(ray)) yield return s;
			}
		}

		public override void SetupForRendering()
		{
			AABB aabb = AABB.Empty;
			foreach(var i in arrayInstances)
			{
				i.SetupForRendering();
				aabb = aabb.Join(i.GetTotalShapeAABB());
			}
		}

		public override AABB GetTotalShapeAABB()
		{
			return totalAABB;
		}
	}
}
