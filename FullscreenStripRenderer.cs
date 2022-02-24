using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	public class FullscreenStripRenderer : ScreenRenderer
	{

		int renderedStrips;
		int totalStrips;

		public override void GetProgressInfo(out string progressString, out float progress)
		{
			progressString = $"{renderedStrips}/{totalStrips}";
			progress = renderedStrips / (float)totalStrips;
		}

		public override void RenderToScreen(Camera camera, Scene scene, byte[] byteBuffer, int width, int height, int pixelDepth)
		{
			renderedStrips = 0;
			totalStrips = height;
			Parallel.For(0, height, i => DrawPixelRow(i, camera, scene, byteBuffer, width, height, pixelDepth));
		}

		private void DrawPixelRow(int y1, Camera camera, Scene scene, byte[] buffer, int width, int height, int depth)
		{
			for (int x = 0; x < width; x++)
			{
				DrawPixel(x, y1, camera, scene, buffer, width, height, depth);
			}
			renderedStrips++;
		}
	}
}
