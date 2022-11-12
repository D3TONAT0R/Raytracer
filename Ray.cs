using System;
using System.Numerics;

namespace Raytracer {
	public class Ray {

		public readonly Vector3 origin;
		public Vector3 position;
		public float maxDistance = 1000;

		private Vector3 dir;
		public Vector3 Inverse {
			get;
			private set;
		}
		public Vector3 Direction {
			get {
				return dir;
			}
			set {
				dir = value;
				Inverse = new Vector3(1 / value.X, 1 / value.Y, 1 / value.Z);
			}
		}

		public Ray(Vector3 pos, Vector3 dir, int iteration, Vector2 screenPos, float maxDistance = 1000) {
			origin = pos;
			position = pos;
			this.dir = dir;
			reflectionIteration = iteration;
			sourceScreenPos = screenPos;
			this.maxDistance = maxDistance;
		}

		public int reflectionIteration;
		public float travelDistance;
		public Vector2 sourceScreenPos;

		public bool Advance(float distance) {
			float advance = Math.Min(maxDistance - travelDistance, distance);
			if(advance < 0.0001f) {
				//Max distance reached
				return false;
			}
			position += Direction * distance;
			travelDistance += distance;
			return true;
		}
	}
}
