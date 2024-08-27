using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	[ObjectIdentifier("INSTANCE")]
	public class SceneObjectInstance : SceneObject, IReferencedObject
	{
		public SceneObject referencedObject;
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
			referencedObject = refObj.Clone();
			referencedObject.parent = this;
			referencedObject.Initialize(parentScene);
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

		public override AABB ComputeLocalShapeBounds()
		{
			return referencedObject.LocalShapeBounds;
		}
	}
}
