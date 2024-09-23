using System;

namespace Raytracer
{
	public class RenderSettings
	{
		public string name;

		public float rayMarchDistanceInVoid = 0.20f;
		public float rayMarchDistanceInObject = 0.02f;
		public float rayDistanceDegradation = 0.1f;

		public bool sampleTextures = true;

		public float bouncedRayQuality = 0.75f;
		public int maxBounces = 3;

		public LightingType lightingType = LightingType.SimpleNormalBased;
		public bool allowSelfShadowing = true;
		public bool specularHighlights = true;

		public RenderSettings(string name)
		{
			this.name = name;
		}

		public RenderSettings Scale(string newName, float scale)
		{
			RenderSettings rs = new RenderSettings(newName);
			rs.rayMarchDistanceInVoid /= scale;
			rs.rayMarchDistanceInObject /= scale;
			bouncedRayQuality /= (float)Math.Sqrt(scale);
			maxBounces = (int)(maxBounces / scale);
			return rs;
		}
	}
}
