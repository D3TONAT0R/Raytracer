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

		public override Color GetColorAt(Vector3 pos, Ray ray) {
			var mat = GetMaterial(pos) ?? RaytracerEngine.Scene.DefaultMaterial;
			return mat.GetColor(this, pos, GetLocalNormalAt(pos), ray);
		}
	}
}
