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

		public Roof(string name, Vector3 pos, Vector3 size, RoofType type, Material roofMat) : base(name, pos, size, false) {
			roofType = type;
			roofMaterial = roofMat;
		}

		protected override void OnInit() {
			base.OnInit();
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

		public override Material GetMaterial(Vector3 pos) {
			CalculateNearestFace(pos, out int face, out _);
			if(face == 0) {
				return material;
			} else if(face == 1) {
				return roofMaterial;
			} else if(face == 2) {
				return cuts[0] > 0 ? roofMaterial : material;
			} else if(face == 3) {
				return cuts[2] > 0 ? roofMaterial : material;
			} else if(face == 4) {
				return cuts[1] > 0 ? roofMaterial : material;
			} else if(face == 5) {
				return cuts[3] > 0 ? roofMaterial : material;
			} else {
				return material;
			}
		}
	}
}
