using System.Numerics;

namespace Raytracer
{
	public class Environment
	{

		public Gradient skyboxGradient;
		public Sampler2D skyboxTexture;
		public bool skyboxIsSpherical;

		public Color ambientColor = new Color(0.2f, 0.2f, 0.25f);
		public Color simpleSunColor = new Color(1.00f, 0.96f, 0.88f);

		public float fogDistance = 250f;
		public Color fogColor = System.Drawing.Color.LightSkyBlue;

		public Environment()
		{
			skyboxGradient = new Gradient(
				(0, System.Drawing.Color.Black),
				(0.49f, System.Drawing.Color.DarkOliveGreen),
				(0.51f, System.Drawing.Color.LightBlue),
				(1f, System.Drawing.Color.White)
			);
		}

		public Color SampleSkybox(Vector3 direction)
		{
			if (skyboxTexture != null)
			{
				if (skyboxIsSpherical)
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
			else
			{
				return skyboxGradient.Evaluate(direction.Y / 2f + 0.5f);
			}
		}
	}
}
