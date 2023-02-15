using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public static class Animator {

		public static float CurrentTime { get; private set; } = 0f;

		public static bool AtEnd => CurrentTime >= Duration;

		public static List<AnimatedProperty> animatedProperties = new List<AnimatedProperty>();

		public static float Duration {
			get {
				float l = 0;
				foreach(var p in animatedProperties) {
					l = Math.Max(l, p.Length);
				}
				return l;
			}
		}

		public static void SetTime(float time)
		{
			CurrentTime = time;
			foreach(var prop in animatedProperties)
			{
				prop.SampleAnimation(CurrentTime);
			}
		}

		public static void Step(float delta)
		{
			SetTime(CurrentTime + delta);
		}
	}
}
