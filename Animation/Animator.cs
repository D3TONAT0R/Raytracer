using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public static class Animator {

		public static float time = 0;

		public static float framesPerSecond = 15;

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

		public static bool Animate() {
			if(time > Duration) return false;
			foreach(var prop in animatedProperties) {
				prop.SampleAnimation(time);
			}
			time += 1f / framesPerSecond;
			return true;
		}
	}
}
