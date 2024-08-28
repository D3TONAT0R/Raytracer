using System;
using System.Numerics;
using System.Windows.Forms;

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

		public float MarchingMultiplier
		{
			get => marchingMultiplier;
			set
			{
				marchingMultiplier = Math.Max(0.01f, marchingMultiplier);
			}
		}

		private float marchingMultiplier = 1f;

		public Ray(Vector3 pos, Vector3 dir, int iteration, Vector2 screenPos, float maxDistance = 1000) {
			origin = pos;
			Direction = dir;
			reflectionIteration = iteration;
			sourceScreenPos = screenPos;
			this.maxDistance = maxDistance;
		}

		public Ray(Ray original)
		{
			origin = original.origin;
			maxDistance = original.maxDistance;
			reflectionIteration = original.reflectionIteration;
			travelDistance = original.travelDistance;
			sourceScreenPos = original.sourceScreenPos;
			Direction = original.Direction;
		}

		public bool Advance(float distance) {
			float advance = Math.Min(maxDistance - travelDistance, distance);
			if(advance < 0.0001f)
			{
				//Max distance reached
				return false;
			}
			travelDistance += distance;
			return true;
		}

		public bool March(bool useObjectMarchDistance, bool noDegradation = false)
		{
			float distance;
			if (useObjectMarchDistance) distance = RaytracerEngine.CurrentRenderSettings.rayMarchDistanceInObject;
			else distance = RaytracerEngine.CurrentRenderSettings.rayMarchDistanceInVoid;
			float degradation = noDegradation ? 0 : travelDistance * RaytracerEngine.CurrentRenderSettings.rayDistanceDegradation;
			return Advance(distance * marchingMultiplier + degradation);
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
