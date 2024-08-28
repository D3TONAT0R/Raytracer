using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public abstract class SolidShape : Shape {

		public SolidShape(string name) : base(name) {

		}

		public override Color GetColorAt(Vector3 localPos, Ray ray) {
			var mat = GetMaterial(localPos) ?? RaytracerEngine.Scene.DefaultMaterial;
			return mat.GetColor(this, localPos, GetLocalNormalAt(localPos), ray);
		}
	}
}
