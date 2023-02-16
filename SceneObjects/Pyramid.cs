using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer.SceneObjects
{
	[ObjectIdentifier("PYRAMID")]
	public class Pyramid : Prism
	{
		public Pyramid() : base()
		{
			cuts = new float[4] { 0.5f, 0.5f, 0.5f, 0.5f };
		}

		public Pyramid(string name, Vector3 position, Vector3 size) : base(name, position, size)
		{
			cuts = new float[4] { 0.5f, 0.5f, 0.5f, 0.5f };
		}

		protected override void OnInit(Scene parentScene)
		{
			cuts = new float[4] { 0.5f, 0.5f, 0.5f, 0.5f };
		}
	}
}
