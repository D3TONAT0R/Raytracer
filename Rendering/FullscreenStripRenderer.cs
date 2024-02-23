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

		public override void RenderToScreen(Camera camera, Scene scene, RenderTarget renderTarget)
		{
			renderedStrips = 0;
			totalStrips = renderTarget.height;
			Parallel.For(0, renderTarget.height, i => DrawPixelRow(i, camera, scene, renderTarget));
		}

		private void DrawPixelRow(int y1, Camera camera, Scene scene, RenderTarget renderTarget)
		{
			try
			{
				for(int x = 0; x < renderTarget.width; x++)
				{
					DrawPixel(x, y1, camera, scene, renderTarget);
				}
				//Console.WriteLine("render row " + y1);
			}
			catch
			{
				Console.WriteLine("fail row " + y1);
				//Catch errors during rendering
			}
			renderedStrips++;
		}
	}
}
