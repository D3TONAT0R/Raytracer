using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	[ObjectIdentifier("INSTANCE")]
	public class ObjectInstance : SceneObject
	{
		public string referencedObjectName;
		public SceneObject referencedObject;
		[DataIdentifier("MATERIAL")]
		public Material overrideMaterial;

		public override Material OverrideMaterial => overrideMaterial ?? parent?.OverrideMaterial;

		public override bool CanContainShapes => true;

		protected override void OnInit()
		{
			var refObj = RaytracerEngine.Scene.GetPrefabOrSceneObject(referencedObjectName);
			if(refObj == null)
			{
				throw new NullReferenceException($"The referenced SceneObject '{referencedObjectName}' does not exist.");
			}
			referencedObject = refObj.Clone();
			referencedObject.parent = this;
			referencedObject.Initialize();
		}

		public override IEnumerable<T> GetContainedObjectsOfType<T>()
		{
			return referencedObject.GetContainedObjectsOfType<T>();
		}

		public override IEnumerable<Shape> GetIntersectingShapes(Ray ray)
		{
			foreach(var s in referencedObject.GetIntersectingShapes(ray)) yield return s;
		}

		public override void SetupForRendering()
		{
			referencedObject.SetupForRendering();
		}

		public override AABB GetTotalShapeAABB()
		{
			return referencedObject.GetTotalShapeAABB();
		}
	}
}
