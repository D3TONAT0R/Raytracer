using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms;

namespace Raytracer {
	public class Ray {

		public readonly Vector3 origin;
		public Vector3 Position => origin + dir * travelDistance;
		public float maxDistance = 1000;

		public int reflectionIteration;
		public float travelDistance;
		public float startDistance;
		public Vector2 sourceScreenPos;

		public Color accumulatedVolumeColor;

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

		public void SetStartDistance(float distance)
		{
			startDistance = distance;
			travelDistance = Math.Max(travelDistance, startDistance);
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

		//TODO: Use shape local space for even better optimization
		public bool AdvanceToNextObjectBounds(List<SceneObject> objects)
		{
			//Jump directly to the first intersection point (skip marching in empty space)
			float nearestIntersection = float.MaxValue;
			float farthestIntersection = travelDistance;
			for(int i = 0; i < objects.Count; i++)
			{
				var shape = objects[i];
				byte hitFlags = GetAABBIntersectionPoints(shape.WorldSpaceShapeBounds, Matrix4x4.Identity, Matrix4x4.Identity, out var p1, out var p2);
				//GetAABBIntersectionPoints(ray, shape.LocalShapeBounds, shape.WorldToLocalMatrix, shape.LocalToWorldMatrix);
				//if((hitFlags & (1 << 0)) != 0)
				if(hitFlags > 0)
				{
					float distance = Vector3.Dot(p1 - Position, Direction);
					if(distance > 0)
					{
						nearestIntersection = Math.Min(nearestIntersection, distance);
					}
				}
				if(hitFlags == 2)
				{
					float distance = Vector3.Dot(p2 - Position, Direction);
					if(distance > 0)
					{
						farthestIntersection = Math.Max(farthestIntersection, distance);
					}
				}
			}
			if(startDistance == 0 && farthestIntersection > 0)
			{
				maxDistance = farthestIntersection;
			}
			if(nearestIntersection < float.MaxValue && nearestIntersection > 0)
			{
				float target = nearestIntersection + 0.00001f;
				if(startDistance == 0)
				{
					SetStartDistance(target);
					return true;
				}
				else
				{
					return Advance(target);
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the intersection points between a ray and a bounding box.
		/// </summary>
		/// <param name="ray">The ray to intersect with the AABB.</param>
		/// <param name="aabb">The AABB to intersect with the ray.</param>
		/// <param name="aabbMatrix">The transformation matrix of the AABB.</param>
		/// <param name="inverseAabbMatrix">The inverse transformation matrix of the AABB.</param>
		/// <returns>The intersection points between the ray and the AABB.</returns>
		public byte GetAABBIntersectionPoints(AABB aabb, Matrix4x4 aabbMatrix, Matrix4x4 inverseAabbMatrix, out Vector3 p1, out Vector3 p2)
		{
			//TODO: breaks shapes when optimizing a ray
			var rpos = Position;
			var rdir = Direction;
			//Matrix4x4.Invert(aabbMatrix, out var invAabbMatrix);
			var ray = Transform(aabbMatrix);
			//var rpos = Vector3.Transform(ray.Position, aabbMatrix);
			//var rdir = Vector3.TransformNormal(ray.Direction, aabbMatrix);

			Vector3 segmentBegin = rpos;
			Vector3 segmentEnd = rpos + rdir * ray.maxDistance;
			Vector3 boxCenter = aabb.Center;
			Vector3 boxSize = aabb.Size;

			var beginToEnd = segmentEnd - segmentBegin;
			var minToMax = new Vector3(boxSize.X, boxSize.Y, boxSize.Z);
			var min = boxCenter - minToMax / 2;
			var max = boxCenter + minToMax / 2;
			var beginToMin = min - segmentBegin;
			var beginToMax = max - segmentBegin;
			var tNear = float.MinValue;
			var tFar = float.MaxValue;

			p1 = Vector3.Zero;
			p2 = Vector3.Zero;

			for(int axis = 0; axis < 3; axis++)
			{
				if(beginToEnd.GetAxisValue(axis) == 0) // parallel
				{
					if(beginToMin.GetAxisValue(axis) > 0 || beginToMax.GetAxisValue(axis) < 0)
						return 0; // segment is not between planes
				}
				else
				{
					var t1 = beginToMin.GetAxisValue(axis) / beginToEnd.GetAxisValue(axis);
					var t2 = beginToMax.GetAxisValue(axis) / beginToEnd.GetAxisValue(axis);
					var tMin = Math.Min(t1, t2);
					var tMax = Math.Max(t1, t2);
					if(tMin > tNear) tNear = tMin;
					if(tMax < tFar) tFar = tMax;
					if(tNear > tFar || tFar < 0) return 0;

				}
			}
			byte hits = 0;
			if(tNear >= 0 && tNear <= 1)
			{
				hits++;
				p1 = Vector3.Transform(segmentBegin + beginToEnd * tNear, inverseAabbMatrix);
			}
			if(tFar >= 0 && tFar <= 1)
			{
				hits++;
				Vector3 v = Vector3.Transform(segmentBegin + beginToEnd * tFar, inverseAabbMatrix);
				if(hits == 1) p1 = v;
				else p2 = v;
			}
			return hits;
		}

		public void AccumulateVolumeColor(Color color)
		{
			if(accumulatedVolumeColor.a <= 0)
			{
				accumulatedVolumeColor = color;
			}
			else
			{
				//TODO: needs testing
				float w = color.a / (color.a + accumulatedVolumeColor.a);
				accumulatedVolumeColor = accumulatedVolumeColor * (1 - w) + color * w;
			}
		}
	}
}
