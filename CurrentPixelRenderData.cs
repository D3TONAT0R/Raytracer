using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public static class CurrentPixelRenderData {

		public static int screenWidth;
		public static int screenHeight;

		[ThreadStatic] public static int pixelX;
		[ThreadStatic] public static int pixelY;

		//[ThreadStatic] public static Vector2 viewportCoord;
	}
}
