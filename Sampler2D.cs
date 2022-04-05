using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class Sampler2D {

		public static string defaultPath = Path.Combine(RaytracerEngine.rootPath, "Textures");

		public readonly string relativeTexturePath;

		public Color[,] texture;
		public readonly int width;
		public readonly int height;
		public bool filtering = true;
		public Color averageColor;

		private Sampler2D(string path, string relativePath) {
			relativeTexturePath = relativePath;
			Bitmap bmp = new Bitmap(Image.FromFile(path));
			width = bmp.Width;
			height = bmp.Height;
			LoadTexture(bmp);
			if(width < 32) {
				filtering = false;
			}
		}

		private void LoadTexture(Bitmap bmp) {
			texture = new Color[width, height];
			/*for(int x = 0; x < width; x++) {
				for(int y = 0; y < height; y++) {
					var c = bmp.GetPixel(x, y);
					texture[x, y] = c;
					averageColor += c;
				}
			}*/
			using(var snoop = new BmpPixelSnoop(bmp)) {
				for(int y = 0; y != snoop.Height; y++) {
					for(int x = 0; x != snoop.Width; x++) {
						Color c = snoop.GetPixel(x, y);
						texture[x, y] = c;
						averageColor += c;
					}
				}
			}
			averageColor *= 1f / (width * height);
		}

		public static Sampler2D Create(string textureName) {
			if(!Path.HasExtension(textureName)) textureName = textureName + ".*";
			//string path = Path.Combine(defaultPath, textureName);
			var files = Directory.GetFiles(defaultPath, Path.GetFileName(textureName));
			if(files.Length > 0) {
				return new Sampler2D(files[0], textureName);
			} else {
				return null;
			}
		}

		public Color Sample(float x, float y, bool alwaysSample = false) {
			while(x < 0) x++;
			while(y < 0) y++;
			if(!RaytracerEngine.CurrentRenderSettings.sampleTextures && !alwaysSample) return averageColor;
			x -= (float)Math.Floor(x);
			y -= (float)Math.Floor(y);
			y = 1 - y;
			if(filtering) {
				int lx = (int)(x * width) % width;
				int ly = (int)(y * height) % height;
				int ux = (lx + 1) % width;
				int uy = (ly + 1) % height;
				float wx = (x * width - lx).Clamp01();
				float wy = (y * height - ly).Clamp01();
				Color c00 = texture[lx, ly];
				Color c01 = texture[lx, uy];
				Color c10 = texture[ux, ly];
				Color c11 = texture[ux, uy];
				Color l = Color.Lerp(c00, c01, wy);
				Color r = Color.Lerp(c10, c11, wy);
				return Color.Lerp(l, r, wx);
			} else {
				int ix = (int)Math.Round(x * width) % width;
				int iy = (int)Math.Round(y * height) % height;
				return texture[ix, iy];
			}
		}

		public override string ToString()
		{
			string s = relativeTexturePath;
			if (s.EndsWith(".*")) s = s.Substring(0, s.Length - 2);
			return s;
		}
	}
}
