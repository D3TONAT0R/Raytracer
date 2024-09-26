using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Raytracer.CurrentPixelRenderData;

namespace Raytracer
{
	public abstract class ScreenRenderer
	{
		public abstract void RenderToScreen(Camera camera, Scene scene, byte[] byteBuffer, int width, int height, int pixelDepth);

		public abstract void GetProgressInfo(out string progressString, out float progress);

		protected void RenderPixel(int x, int y, Camera camera, Scene scene, byte[] buffer, int width, int height, int depth)
		{
			var color = TracePixel(x, y, camera, scene);
			SetPixel(buffer, x, y, color, width, height, depth);
		}

		protected Color TracePixel(int x, int y, Camera camera, Scene scene)
		{
			var viewportCoord = new Vector2(x / (float)screenWidth, y / (float)screenHeight) * 2f - Vector2.One;
			pixelX = x;
			pixelY = y;
			var ray = camera.ScreenPointToRay(viewportCoord);
			Color c = SceneRenderer.TraceRay(scene, ray, VisibilityFlags.Direct).SetAlpha(1);
			switch(RaytracerEngine.Mode)
			{
				case RenderMode.Normal: return c;
				case RenderMode.BoundingBoxesDebug:
					float d = Math.Max(ray.startDistance / 10f, 0);
					return new Color(d, d, d, 1);
				default: return new Color(1, 0, 1, 1);
			}
		}

		protected void SetPixel(byte[] buffer, int x, int y, Color color, int width, int height, int depth)
		{
			SetPixelBytes(buffer, x, y, color.GetBytes(), width, height, depth);
		}

		protected void FillPixels(byte[] buffer, int x1, int y1, int x2, int y2, Color color, int width, int height, int depth)
		{
			x2 = Math.Min(x2, width - 1);
			y2 = Math.Min(y2, height - 1);
			var cBytes = color.GetBytes();
			for(int y = y1; y <= y2; y++)
			{
				for (int x = x1; x <= x2; x++)
				{
					SetPixelBytes(buffer, x, y, cBytes, width, height, depth);
				}
			}
		}

		private void SetPixelBytes(byte[] buffer, int x, int y, byte[] color, int width, int height, int depth)
		{
			int by = height - y - 1;
			int pos = (by * width + x) * depth;

			buffer[pos + 0] = color[2]; //B
			buffer[pos + 1] = color[1]; //G
			buffer[pos + 2] = color[0]; //R
		}
	}
}
