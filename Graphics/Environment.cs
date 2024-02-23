using System.Numerics;

namespace Raytracer
{
	[ObjectIdentifier("ENVIRONMENT")]
	public class Environment
	{
		[DataIdentifier("SKY_GRADIENT")]
		public Gradient skyboxGradient;
		[DataIdentifier("SKYBOX_TEX")]
		public Sampler2D skyboxTexture;
		[DataIdentifier("SKYBOX_SPHERICAL")]
		public bool skyboxIsSpherical = true;

		[DataIdentifier("SKY_TINT")]
		public Color skyboxTint = Color.White;
		[DataIdentifier("SKY_BRIGHTNESS", 0.01f)]
		public float skyboxBrightness = 1.0f;

		[DataIdentifier("AMBIENT_COLOR")]
		public Color ambientColor = new Color(0.2f, 0.2f, 0.25f);
		[DataIdentifier("AMBIENT_BRIGHTNESS", 0.01f)]
		public float ambientBrightness = 1f;
		public Color simpleSunColor = new Color(1.00f, 0.96f, 0.88f);

		[DataIdentifier("FOG_DISTANCE", 1f)]
		public float fogDistance = 250f;
		[DataIdentifier("FOG_COLOR")]
		public Color fogColor = System.Drawing.Color.LightSkyBlue;

		public Color AmbientLight => ambientColor * ambientBrightness;

		public Environment()
		{
			skyboxGradient = new Gradient(
				new GradientKey(0, System.Drawing.Color.Black),
				new GradientKey(0.49f, System.Drawing.Color.DarkOliveGreen),
				new GradientKey(0.51f, System.Drawing.Color.LightBlue),
				new GradientKey(1f, System.Drawing.Color.White)
			);
		}

		public Color SampleSkybox(Vector3 direction)
		{
			return SampleSky(direction) * skyboxTint * skyboxBrightness;
		}

		private Color SampleSky(Vector3 direction)
		{
			if(skyboxTexture != null)
			{
				if(skyboxIsSpherical)
				{
					var x = MathUtils.DirToEuler(direction).Y / 360f;
					var y = 0.5f + direction.Y / 2f;
					return skyboxTexture.Sample(x, y);
				}
				else
				{
					var euler = MathUtils.DirToEuler(direction);
					var x = euler.Y / 90f;
					x += 0.5f;
					var y = euler.X / 90f;
					y += 0.5f;
					return skyboxTexture.Sample(x, y);
				}
			}
			else if(skyboxGradient != null)
			{
				return skyboxGradient.Evaluate(direction.Y / 2f + 0.5f);
			}
			else
			{
				return Color.Gray;
			}
		}
	}
}
