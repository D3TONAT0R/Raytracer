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
		[DataIdentifier("OFFSET", 0.1f)]
		public Vector3 offset = new Vector3(0, 0, 1);
		[DataIdentifier("COUNT")]
		public int instanceCount = 1;
		[DataIdentifier("MATERIAL")]
		public Material overrideMaterial;

		public string ReferencedObjectName { get; set; }

		public override Material OverrideMaterial => overrideMaterial ?? parent?.OverrideMaterial;

		public override bool CanContainShapes => true;

		protected override void OnInit(Scene parentScene)
		{
			var refObj = parentScene.GetPrefabOrSceneObject(ReferencedObjectName);
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
				inst.Initialize(parentScene);
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

		public override AABB ComputeLocalShapeBounds()
		{
			/*
			foreach (var arrayInstance in arrayInstances)
			{
				arrayInstance.RecalculateShapeBounds();
			}
			*/

			if(arrayInstances.Length == 0) return AABB.NullBounds;
			var bounds = arrayInstances[0].ComputeLocalShapeBounds();
			for (int i = 1; i < arrayInstances.Length; i++)
			{
				bounds = bounds.Join(arrayInstances[i].ComputeLocalShapeBounds());
			}
			return bounds;
		}

		public override void SetupForRendering()
		{
			
		}
	}
}
