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

		public override Color GetColorAt(Vector3 localPos, Ray ray, bool invertNormals = false) {
			var mat = GetMaterial(localPos) ?? RaytracerEngine.Scene.DefaultMaterial;
			var normal = GetLocalNormalAt(localPos);
			if (invertNormals) normal *= -1;
			return mat.GetColor(this, localPos, normal, ray);
		}
	}
}
