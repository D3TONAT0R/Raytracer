using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Raytracer.RaytracerEngine;

namespace Raytracer {

	[ObjectIdentifier("TERRAIN")]
	public class Terrain : SolidShape {

		[DataIdentifier("SIZE")]
		public Vector3 dimensions = new Vector3(10, 1, 10);
		[DataIdentifier("BOTTOM")]
		public float bottomThickness = 1;

		[DataIdentifier("NRMSMOOTHNESS")]
		public float normalSmoothness = 0.001f;

		public Sampler2D heightmap;
		[DataIdentifier("HEIGHTMAP")]
		public string heightmapFile;

		public Terrain() : base(null) { }

		public Terrain(string name, Vector3 pos, Vector3 size, float thickness, string heightmap, Material mat) : base(name) {
			localPosition = pos;
			dimensions = size;
			bottomThickness = thickness;
			heightmapFile = heightmap;
			material = mat;
		}

		protected override void OnInit() {
			heightmap = Sampler2D.Create(heightmapFile, RaytracerEngine.Scene.rootDirectory);
			if (material != null)
			{
				material.mappingType = TextureMappingType.LocalYProj;
				material.textureTiling = new TilingVector(0, 0, 1f / dimensions.X, 1f / dimensions.Z);
			}
		}

		public override void SetupForRendering() {
			var add = Vector3.UnitY * bottomThickness;
			ShapeAABB = new AABB(WorldPosition - add, WorldPosition + dimensions + add);
		}

		public override Vector3 GetNormalAt(Vector3 pos, Ray ray) {
			return CalculateNormal(GetTerrainCoord(pos).XZ(), true);
		}

		public override bool Intersects(Vector3 pos) {
			var norm = GetTerrainCoord(pos);
			return GetHeightAt(norm.XZ()) >= norm.Y;
		}

		public Vector3 GetTerrainCoord(Vector3 worldPos) {
			var pos = worldPos - localPosition;
			pos /= dimensions;
			return pos;
		}

		public float GetHeightAt(float x, float y) {
			return heightmap.Sample(x, y).r;
		}

		public float GetHeightAt(Vector2 posNorm) {
			return GetHeightAt(posNorm.X, posNorm.Y);
		}

		private float GetSlope(float from, float to, float dst) {
			float hdiff = to - from;
			return (float)(MathUtils.Rad2Deg * Math.Atan(hdiff / dst) / 90f);
		}

		private Vector3 CalculateNormal(Vector2 posNorm, bool sharpMode) {
			float x = posNorm.X;
			float y = posNorm.Y;
			float o = Math.Max(0.001f, normalSmoothness);
			//float o2 = o * 2f;
			if(sharpMode) {
				float ll = GetHeightAt(x, y);
				float lr = GetHeightAt(x + o, y);
				float ul = GetHeightAt(x, y + o);
				float ur = GetHeightAt(x + o, y + o);
				float nrmX = (GetSlope(lr, ll, o) + GetSlope(ur, ul, o)) / 2f;
				float nrmY = (GetSlope(ul, ll, o) + GetSlope(ur, lr, o)) / 2f;
				float power = Math.Abs(nrmX) + Math.Abs(nrmY);
				if(power > 1) {
					nrmX /= power;
					nrmY /= power;
				}
				float nrmZ = 1f - power;
				return Vector3.Normalize(new Vector3(nrmX, nrmZ, nrmY));
			} else {
				/*normals = new Vector3[grid.GetLength(0) - 1, grid.GetLength(1) - 1];
				for(int x = 0; x < image.Width; x++) {
					for(int y = 0; y < image.Height; y++) {
						float m = GetHeightAt(x, y);
						float r = GetSlope(GetHeightAt(x + 1, y), m);
						float l = GetSlope(m, GetHeightAt(x - 1, y));
						float u = GetSlope(GetHeightAt(x, y + 1), m);
						float d = GetSlope(m, GetHeightAt(x, y - 1));
						float nrmX = (r + l) / 2f;
						float nrmY = (u + d) / 2f;
						float power = Math.Abs(nrmX) + Math.Abs(nrmY);
						if(power > 1) {
							nrmX /= power;
							nrmY /= power;
						}
						float nrmZ = 1f - power;
						normals[x, y] = Normalize(new Vector3(nrmX, nrmY, nrmZ));
					}
				}*/
				return Vector3.UnitY;
			}
		}

		public override float GetSurfaceProximity(Vector3 worldPos) {
			var h = GetHeightAt(GetTerrainCoord(worldPos).XZ());
			h *= dimensions.Y;
			h += localPosition.Y;
			return Math.Abs(h - worldPos.Y);
		}

		public override Vector2 GetUV(Vector3 localPos, Vector3 normal)
		{
			return new Vector2(localPos.X / dimensions.X, localPos.Z / dimensions.Z);
		}
	}
}
