using System;
using System.Numerics;

namespace Raytracer {
	public class Ray {

		public readonly Vector3 origin;
		public Vector3 Position => origin + dir * travelDistance;
		public float maxDistance = 1000;

		public int reflectionIteration;
		public float travelDistance;
		public Vector2 sourceScreenPos;

		private Vector3 dir;
		public Vector3 InverseDirection {
			get;
			private set;
		}
		public Vector3 Direction {
			get {
				return dir;
			}
			set {
				dir = value;
				InverseDirection = new Vector3(1 / value.X, 1 / value.Y, 1 / value.Z);
			}
		}

		public Ray(Vector3 pos, Vector3 dir, int iteration, Vector2 screenPos, float maxDistance = 1000) {
			origin = pos;
			Direction = dir;
			reflectionIteration = iteration;
			sourceScreenPos = screenPos;
			this.maxDistance = maxDistance;
		}

		public bool Advance(float distance) {
			float advance = Math.Min(maxDistance - travelDistance, distance);
			if(advance < 0.0001f) {
				//Max distance reached
				return false;
			}
			travelDistance += distance;
			return true;
		}

		public Ray Transform(Matrix4x4 matrix)
		{
			return new Ray(Vector3.Transform(origin, matrix), Vector3.Normalize(Vector3.TransformNormal(Direction, matrix)), reflectionIteration, sourceScreenPos, maxDistance)
			{
				travelDistance = travelDistance
			};
		}
	}
}
