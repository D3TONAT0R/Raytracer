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
		public abstract void RenderToScreen(Camera camera, Scene scene, RenderTarget renderTarget);

		public abstract void GetProgressInfo(out string progressString, out float progress);

		protected void DrawPixel(int x, int y, Camera camera, Scene scene, RenderTarget renderTarget)
		{
			var color = GetPixelColor(x, y, camera, scene);
			renderTarget.WritePixel(x, y, color);
		}

		protected Color GetPixelColor(int x, int y, Camera camera, Scene scene)
		{
			var viewportCoord = new Vector2(x / (float)screenWidth, y / (float)screenHeight) * 2f - Vector2.One;
			pixelX = x;
			pixelY = y;

			var ray = camera.ScreenPointToRay(viewportCoord);
			return SceneRenderer.TraceRay(scene, ray, VisibilityFlags.Direct).SetAlpha(1);
		}
	}
}
