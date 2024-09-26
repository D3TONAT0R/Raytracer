using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class Sampler2D {

		public static string defaultPath = Path.Combine(PersistentPrefs.ResourcesRootPath, "Textures");

		public readonly string textureName;

		public Color[,] texture;
		public readonly int width;
		public readonly int height;
		[DataIdentifier("FILTER")]
		public bool filtering = true;
		public Color averageColor;

		private Sampler2D(string path, bool bilinearFilter) {
			textureName = Path.GetFileName(path);
			Bitmap bmp = new Bitmap(Image.FromFile(path));
			width = bmp.Width;
			height = bmp.Height;
			LoadTextureData(bmp);
			filtering = bilinearFilter;
		}

		private void LoadTextureData(Bitmap bmp) {
			texture = new Color[width, height];
			/*
			using(var snoop = new BmpPixelSnoop(bmp)) {
				for(int y = 0; y != snoop.Height; y++) {
					for(int x = 0; x != snoop.Width; x++) {
						Color c = snoop.GetPixel(x, y);
						texture[x, y] = c;
						averageColor += c;
					}
				}
			}
			*/
			var data = bmp.LockBits(new Rectangle(0,0,bmp.Width,bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			var size = data.Stride * data.Height;
			var bytes = new byte[size];
			Marshal.Copy(data.Scan0, bytes, 0, size);
			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{
					int i = (y * width + x) * 4;
					Color c = new Color(bytes[i + 2] / 255f, bytes[i + 1] / 255f, bytes[i + 0] / 255f, bytes[i + 3] / 255f);
					texture[x, y] = c;
					averageColor += c;
				}
			}
			averageColor *= 1f / (width * height);
		}

		public static Sampler2D Create(string input, string sceneRootPath) {

			SceneFileLoader.ParseArgumentedInput(input, out input, out var args);
			if(!Path.HasExtension(input)) input = input + ".*";
			var textureFile = LocateTexture(input, sceneRootPath);
			bool filtering = true;
			if(args.Contains("point"))
			{
				filtering = false;
			}
			if(textureFile != null) {
				return new Sampler2D(textureFile, filtering);
			} else {
				return null;
			}
		}

		private static string LocateTexture(string textureFileName, string sceneRootPath)
		{
			string[] files;
			if(sceneRootPath != null)
			{
				files = Directory.GetFiles(sceneRootPath, textureFileName);
				if(files.Length > 0) return files[0];
			}
			if(Directory.Exists(defaultPath))
			{
				files = Directory.GetFiles(defaultPath, textureFileName);
				if(files.Length > 0) return files[0];
			}
			return null;
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
			string s = textureName;
			if (s.EndsWith(".*")) s = s.Substring(0, s.Length - 2);
			return s;
		}
	}
}
