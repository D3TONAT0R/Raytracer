using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class RenderSettings {

		public int screenWidth = 128;
		public int screenHeight = 72;

		public int PixelCount => screenWidth * screenHeight;

		public float rayMarchDistanceInVoid = 0.20f;
		public float rayMarchDistanceInObject = 0.02f;
		public float rayDistanceDegradation = 0.1f;

		public bool sampleTextures = true;

		public float bouncedRayQuality = 0.75f;
		public int maxBounces = 3;

		public LightingType lightingType = LightingType.SimpleNormalBased;
		public bool allowSelfShadowing = true;
		public bool specularHighlights = true;

		private Bitmap m_renderBuffer;
		public Bitmap renderBuffer {
			get {
				if(m_renderBuffer == null) {
					m_renderBuffer = new Bitmap(screenWidth, screenHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				}
				return m_renderBuffer;
			}
		}

		public RenderSettings(int width, int height) {
			screenWidth = width;
			screenHeight = height;
		}

		public RenderSettings Scale(float scale) {
			RenderSettings rs = new RenderSettings((int)(screenWidth * scale), (int)(screenHeight * scale));
			rs.rayMarchDistanceInVoid /= scale;
			rs.rayMarchDistanceInObject /= scale;
			bouncedRayQuality /= (float)Math.Sqrt(scale);
			maxBounces = (int)(maxBounces / scale);
			return rs;
		}
	}
}
