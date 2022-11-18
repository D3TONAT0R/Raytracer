using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneObjects
{
	[ObjectIdentifier("WINDOW")]
	public class Window : SceneObject
	{
		[DataIdentifier("SIZE")]
		public Vector3 size;
		[DataIdentifier("AXIS")]
		public Axis axis;
		[DataIdentifier("BORDER")]
		public float border;
		[DataIdentifier("FILLMATERIAL")]
		public Material fillMaterial;

		private Cuboid[] borderCubes;
		private Cuboid fillCube;

		protected override void OnInit()
		{
			base.OnInit();
		}

		public override void SetupForRendering()
		{
			throw new NotImplementedException();
		}
	}
}
