using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("ROOF")]
	public class Roof : Prism {

		public enum RoofType {
			GableX,
			GableZ,
			Hipped,
			Pyramid
		}

		[DataIdentifier("ROOFTYPE")]
		public RoofType roofType;
		[DataIdentifier("ROOFMATERIAL")]
		public Material roofMaterial;

		public Roof() : base() { }

		public Roof(string name, Vector3 pos, Vector3 size, RoofType type, Material roofMat) : base(name, pos, size) {
			roofType = type;
			roofMaterial = roofMat;
		}

		protected override void OnInit(Scene parentScene)
		{
			base.OnInit(parentScene);
			if(roofType == RoofType.GableX) {
				cuts = new float[4] { 0, 0.5f, 0, 0.5f };
			} else if(roofType == RoofType.GableZ) {
				cuts = new float[4] { 0.5f, 0, 0.5f, 0 };
			} else if(roofType == RoofType.Hipped) {
				if(size.X > size.Z) {
					float v = (size.Z / size.X) / 2f;
					cuts = new float[4] { v, 0.5f, v, 0.5f };
				} else {
					float v = (size.X / size.Z) / 2f;
					cuts = new float[4] { 0.5f, v, 0.5f, v };
				}
			} else if(roofType == RoofType.Pyramid) {
				cuts = new float[4] { 0.5f, 0.5f, 0.5f, 0.5f };
			} else {
				cuts = new float[4] { 0, 0, 0, 0 };
			}
		}

		public override Material GetMaterial(Vector3 localPos) {
			CalculateNearestFace(localPos, out int face, out _);
			var r = roofMaterial ?? material;
			Material mat;
			if(face == 0) {
				mat = material;
			} else if(face == 1) {
				mat = r;
			} else if(face == 2) {
				mat = cuts[0] > 0 ? r : material;
			} else if(face == 3) {
				mat = cuts[2] > 0 ? r : material;
			} else if(face == 4) {
				mat = cuts[1] > 0 ? r : material;
			} else if(face == 5) {
				mat = cuts[3] > 0 ? r : material;
			} else {
				mat = material;
			}
			return mat ?? OverrideMaterial;
		}
	}
}
