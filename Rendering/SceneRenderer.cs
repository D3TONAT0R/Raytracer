using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Raytracer
{
	public static class SceneRenderer
	{
		public static bool progressiveUpdates = true;

		public static ScreenRenderer ActiveScreenRenderer
		{
			get
			{
				return progressiveUpdates && RaytracerEngine.instance.IsRendering ? progressiveRenderer : stripRenderer;
			}
		}

		static ScreenRenderer stripRenderer = new FullscreenStripRenderer();
		static ScreenRenderer progressiveRenderer = new ProgressiveChunkRenderer();

		public static object bufferLock = new object();

		static Bitmap currentTarget;
		static BitmapData currentBitmapData;
		static byte[] currentByteBuffer;

		public static bool IsRendering { get; private set; }

		public static void RenderScene(Camera camera, Scene scene, Bitmap target)
		{
			IsRendering = true;
			currentTarget = target;
			var pixelDepth = Bitmap.GetPixelFormatSize(target.PixelFormat) / 8;

			Try(() => BeginCopy(pixelDepth));


			ActiveScreenRenderer.RenderToScreen(camera, scene, currentByteBuffer, currentTarget.Width, currentTarget.Height, pixelDepth);

			Try(() => FlushCurrent());

			currentBitmapData = null;
			currentByteBuffer = null;
			IsRendering = false;
		}

		private static void Try(Action a, int sleep = 10)
		{
			int attemptsLeft = 20;
			while(attemptsLeft > 0)
			{
				attemptsLeft--;
				try
				{
					a.Invoke();
					return;
				}
				catch(Exception e)
				{
					if(attemptsLeft <= 0) throw e;
					Thread.Sleep(sleep);
				}
			}
		}

		private static void BeginCopy(int pixelDepth)
		{
			currentByteBuffer = new byte[currentTarget.Width * currentTarget.Height * pixelDepth];
			lock (bufferLock)
			{
				Lock();
				Marshal.Copy(currentBitmapData.Scan0, currentByteBuffer, 0, currentByteBuffer.Length);
				Unlock();
			}
		}

		public static void FlushCurrent()
		{
			lock (bufferLock)
			{
				bool didLock = false;
				try
				{
					Lock();
					didLock = true;
					Marshal.Copy(currentByteBuffer, 0, currentBitmapData.Scan0, currentByteBuffer.Length);
				}
				catch
				{
					Console.WriteLine("Failed to lock bitmap.");
				}
				finally
				{
					if(didLock) Unlock();
				}
			}
		}

		private static void Lock()
		{
			var rect = new Rectangle(0, 0, currentTarget.Width, currentTarget.Height);
			currentBitmapData = currentTarget.LockBits(rect, ImageLockMode.ReadWrite, currentTarget.PixelFormat);
		}

		private static void Unlock()
		{
			currentTarget.UnlockBits(currentBitmapData);
		}

		public static void RequestImageRefresh()
		{
			FlushCurrent();
		}

		public static Color TraceRay(Scene scene, Ray ray, VisibilityFlags rayType, Shape excludeShape = null, bool allowOptimization = true)
		{
			if (ray.reflectionIteration >= RaytracerEngine.CurrentRenderSettings.maxBounces + 1) return Color.Black;
			Vector3? hit = TraceRay(scene, ref ray, rayType, out var intersection, excludeShape, null, allowOptimization);
			if (hit != null && intersection != null)
			{
				return intersection.GetColorAt((Vector3)hit, ray);
			}
			else
			{
				//We didn't hit anything, render the sky instead
				return scene.environment.SampleSkybox(ray.Direction);
			}
		}

		public static Vector3? TraceRay(Scene scene, ref Ray ray, VisibilityFlags rayType, out Shape intersectingShape, Shape excludeShape = null, Shape exitShape = null, bool allowOptimization = true)
		{
			var shapes = scene.GetIntersectingShapes(ray, rayType);
			intersectingShape = null;
			//Ignore excluded shape
			if (excludeShape != null && shapes.Contains(excludeShape)) shapes.Remove(excludeShape);
			if (shapes.Count > 0)
			{
				//TODO: optimization is temporarily disabled on reflections, causes floating reflections at intersections
				if(allowOptimization) OptimizeRay(ray, shapes);
				while (scene.IsInWorldBounds(ray.Position))
				{
					var intersecting = scene.GetAABBIntersectingShapes(ray.Position, shapes);
					if (intersecting.Length == 0)
					{
						//No AABB collision detected
						if(exitShape != null) return ray.Position;
						if (!ray.Advance(RaytracerEngine.CurrentRenderSettings.rayMarchDistanceInVoid + ray.travelDistance * RaytracerEngine.CurrentRenderSettings.rayDistanceDegradation))
						{
							return ray.Position;
						}
					}
					else
					{
						for (int i = 0; i < intersecting.Length; i++)
						{
							var localpos = ray.Position;
							if (intersecting[i].Intersects(localpos))
							{
								//We are about to hit something
								intersectingShape = intersecting[i];
								return ray.Position;
							}
						}

						var maxReached = !ray.Advance(RaytracerEngine.CurrentRenderSettings.rayMarchDistanceInObject + ray.travelDistance * RaytracerEngine.CurrentRenderSettings.rayDistanceDegradation);
						//If Advance returns false, we have reached the ray's maximum distance without hitting any surface
						if (maxReached)
						{
							return ray.Position;
						}

						if(exitShape != null && exitShape.Intersects(ray.Position)) 
						{
							//We are no longer in contact with the given shape, return the current position instead
							if(ray.travelDistance == 0) throw new InvalidOperationException("Not in contact with target shape after a distance of 0 units.");
							return ray.Position;
						}
					}
				}
			}
			else
			{
				ray.Advance(ray.maxDistance);
				return ray.Position;
			}
			return ray.Position;
		}

		static void OptimizeRay(Ray ray, List<Shape> shapes)
		{
			//Jump directly to the first intersection point (skip marching in empty space)
			float nearestIntersection = float.MaxValue;
			float farthestIntersection = 0;
			for (int i = 0; i < shapes.Count; i++)
			{
				var intersections = GetAABBIntersectionPoints(ray, shapes[i].ExpandedAABB, shapes[i].WorldToLocalMatrix);
				if (intersections.Count > 0)
				{
					nearestIntersection = Math.Min(nearestIntersection, Vector3.Distance(ray.Position, intersections[0]));
				}
				if (intersections.Count > 1)
				{
					farthestIntersection = Math.Max(farthestIntersection, Vector3.Distance(ray.Position, intersections[1]));
				}
			}
			if (farthestIntersection > 0)
			{
				ray.maxDistance = farthestIntersection;
			}
			if (nearestIntersection < float.MaxValue && nearestIntersection > 0)
			{
				ray.Advance(nearestIntersection);
			}
		}

		public static List<Vector3> GetAABBIntersectionPoints(Ray ray, AABB aabb, Matrix4x4 aabbMatrix)
		{
			//TODO: breaks shapes
			var rpos = -ray.Position;
			var rdir = ray.Direction;
			ray = ray.Transform(aabbMatrix);
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

			var intersections = new List<Vector3>();
			for (int axis = 0; axis < 3; axis++)
			{
				if (beginToEnd.GetAxisValue(axis) == 0) // parallel
				{
					if (beginToMin.GetAxisValue(axis) > 0 || beginToMax.GetAxisValue(axis) < 0)
						return intersections; // segment is not between planes
				}
				else
				{
					var t1 = beginToMin.GetAxisValue(axis) / beginToEnd.GetAxisValue(axis);
					var t2 = beginToMax.GetAxisValue(axis) / beginToEnd.GetAxisValue(axis);
					var tMin = Math.Min(t1, t2);
					var tMax = Math.Max(t1, t2);
					if (tMin > tNear) tNear = tMin;
					if (tMax < tFar) tFar = tMax;
					if (tNear > tFar || tFar < 0) return intersections;

				}
			}
			if (tNear >= 0 && tNear <= 1) intersections.Add(segmentBegin + beginToEnd * tNear);
			if (tFar >= 0 && tFar <= 1) intersections.Add(segmentBegin + beginToEnd * tFar);
			return intersections;
		}
	}
}
