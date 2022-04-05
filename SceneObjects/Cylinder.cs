using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[ObjectIdentifier("CYLINDER")]
	public class Cylinder : SolidShape {

		public enum CylinderAxis {
			X, Y, Z
		}

		[DataIdentifier("AXIS")]
		public CylinderAxis axis = CylinderAxis.Y;
		[DataIdentifier("RADIUS")]
		public float radius = 1;
		[DataIdentifier("LENGTH")]
		public float length = 1;

		public Cylinder() : base(null) { }

		public Cylinder(string name, Vector3 pos, float r, float l, CylinderAxis direction = CylinderAxis.Y) : base(name) {
			localPosition = pos;
			radius = r;
			length = l;
			axis = direction;
		}

		public override void SetupAABBs() {
			Vector3 lower = WorldPosition;
			Vector3 upper = WorldPosition;
			if(axis == CylinderAxis.Y) {
				lower.X -= radius;
				lower.Z -= radius;
				upper.X += radius;
				upper.Z += radius;
				upper.Y += length;
			} else if(axis == CylinderAxis.X) {
				lower.Z -= radius;
				lower.Y -= radius;
				upper.Z += radius;
				upper.Y += radius;
				upper.X += length;
			} else if(axis == CylinderAxis.Z) {
				lower.X -= radius;
				lower.Y -= radius;
				upper.X += radius;
				upper.Y += radius;
				upper.Z += length;
			}
			ShapeAABB = new AABB(lower, upper);
		}

		public override float GetSurfaceProximity(Vector3 worldPos) {
			CalculateClosestFace(worldPos, out _, out float prox);
			return prox;
		}

		public override bool Intersects(Vector3 pos) {
			if(axis == CylinderAxis.Y) {
				return pos.Y.Range(ShapeAABB.lower.Y, ShapeAABB.upper.Y) && Vector2.Distance(pos.XZ(), WorldPosition.XZ()) < radius;
			} else if(axis == CylinderAxis.X) {
				return pos.X.Range(ShapeAABB.lower.X, ShapeAABB.upper.X) && Vector2.Distance(pos.ZY(), WorldPosition.ZY()) < radius;
			} else if(axis == CylinderAxis.Z) {
				return pos.Z.Range(ShapeAABB.lower.Z, ShapeAABB.upper.Z) && Vector2.Distance(pos.XY(), WorldPosition.XY()) < radius;
			}
			return true;
		}

		public override Vector3 GetNormalAt(Vector3 pos, Ray ray) {
			CalculateClosestFace(pos, out int face, out _);
			if(axis == CylinderAxis.Y) {
				if(face == 0) {
					var xz = pos.XZ() - WorldPosition.XZ();
					return Vector3.Normalize(new Vector3(xz.X, 0, xz.Y));
				} else if(face == -1) {
					return -Vector3.UnitY;
				} else if(face == 1) {
					return -Vector3.UnitY;
				}
			} else if(axis == CylinderAxis.X) {
				if(face == 0) {
					var zy = pos.ZY() - WorldPosition.ZY();
					return Vector3.Normalize(new Vector3(zy.X, zy.Y, 0));
				} else if(face == -1) {
					return -Vector3.UnitX;
				} else if(face == 1) {
					return Vector3.UnitX;
				}
			} else if(axis == CylinderAxis.Z) {
				if(face == 0) {
					var xy = pos.XY() - WorldPosition.XY();
					return Vector3.Normalize(new Vector3(xy.X, xy.Y, 0));
				} else if(face == -1) {
					return -Vector3.UnitZ;
				} else if(face == 1) {
					return Vector3.UnitZ;
				}
			}
			return Vector3.UnitY;
		}

		private void CalculateClosestFace(Vector3 worldPos, out int face, out float prox) {
			face = 0;
			prox = 0;
			float proxL = 0;
			float proxU = 0;
			if(axis == CylinderAxis.Y) {
				prox = Math.Abs(Vector2.Distance(worldPos.XZ(), WorldPosition.XZ()) - radius);
				proxL = Math.Abs(worldPos.Y - ShapeAABB.lower.Y);
				proxU = Math.Abs(worldPos.Y - ShapeAABB.upper.Y);
			} else if(axis == CylinderAxis.X) {
				prox = Math.Abs(Vector2.Distance(worldPos.ZY(), WorldPosition.ZY()) - radius);
				proxL = Math.Abs(worldPos.X - ShapeAABB.lower.X);
				proxU = Math.Abs(worldPos.X - ShapeAABB.upper.X);
			} else if(axis == CylinderAxis.Z) {
				prox = Math.Abs(Vector2.Distance(worldPos.XY(), WorldPosition.XY()) - radius);
				proxL = Math.Abs(worldPos.Z - ShapeAABB.lower.Z);
				proxU = Math.Abs(worldPos.Z - ShapeAABB.upper.Z);
			}
			if(proxL < prox) {
				face = -1;
				prox = proxL;
			}
			if(proxU < prox) {
				face = 1;
				prox = proxU;
			}
		}
	}
}
