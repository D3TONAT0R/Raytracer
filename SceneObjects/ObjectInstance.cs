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
	}
}
