using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
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

			BeginCopy(pixelDepth);

			ActiveScreenRenderer.RenderToScreen(camera, scene, currentByteBuffer, currentTarget.Width, currentTarget.Height, pixelDepth);

			FlushCurrent();

			currentBitmapData = null;
			currentByteBuffer = null;
			IsRendering = false;
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

		public static Color TraceRay(Scene scene, Ray ray, Shape excludeShape = null)
		{
			if (ray.reflectionIteration >= RaytracerEngine.CurrentRenderSettings.maxBounces + 1) return Color.Black;
			Vector3? hit = TraceRay(scene, ref ray, out var intersection, excludeShape);
			if (hit != null)
			{
				return intersection.GetColorAt((Vector3)hit, ray);
			}
			else
			{
				//We didn't hit anything, render the sky instead
				return scene.environment.SampleSkybox(ray.Direction);
			}
		}

		public static Vector3? TraceRay(Scene scene, ref Ray ray, out Shape intersectingShape, Shape excludeShape = null)
		{
			var shapes = scene.GetIntersectingShapes(ray);
			intersectingShape = null;
			//Ignore excluded shape
			if (excludeShape != null && shapes.Contains(excludeShape)) shapes.Remove(excludeShape);
			if (shapes.Count > 0)
			{
				OptimizeRay(ray, shapes);
				while (scene.IsInWorldBounds(ray.position))
				{
					var intersecting = scene.GetAABBIntersectingShapes(ray.position, shapes);
					if (intersecting.Length == 0)
					{
						//No AABB collision detected
						if (!ray.Advance(RaytracerEngine.CurrentRenderSettings.rayMarchDistanceInVoid + ray.travelDistance * RaytracerEngine.CurrentRenderSettings.rayDistanceDegradation))
						{
							return null;
						}
					}
					else
					{
						for (int i = 0; i < intersecting.Length; i++)
						{
							//TODO: correct?
							//var localpos = ray.position - intersecting[i].HierarchyPositionOffset;
							var localpos = ray.position;
							if (intersecting[i].Intersects(localpos))
							{
								//We are about to hit something
								intersectingShape = intersecting[i];
								return ray.position;
							}
						}
						//If Advance returns false, we have reached the ray's maximum distance without hitting any surface
						if (!ray.Advance(RaytracerEngine.CurrentRenderSettings.rayMarchDistanceInObject + ray.travelDistance * RaytracerEngine.CurrentRenderSettings.rayDistanceDegradation))
						{
							return null;
						}
					}
				}
			}
			return null;
		}

		static void OptimizeRay(Ray ray, List<Shape> shapes)
		{
			//Jump directly to the first intersection point (skip marching in empty space)
			float nearestIntersection = float.MaxValue;
			float farthestIntersection = 0;
			for (int i = 0; i < shapes.Count; i++)
			{
				var intersections = GetAABBIntersectionPoints(ray, shapes[i].ExpandedAABB/*scene.shapeAABBs[shapes[i]][1]*/);
				if (intersections.Count > 0)
				{
					nearestIntersection = Math.Min(nearestIntersection, Vector3.Distance(ray.position, intersections[0]));
				}
				if (intersections.Count > 1)
				{
					farthestIntersection = Math.Max(farthestIntersection, Vector3.Distance(ray.position, intersections[1]));
				}
			}
			if (farthestIntersection > 0)
			{
				ray.maxDistance = farthestIntersection;
			}
			if (nearestIntersection < float.MaxValue)
			{
				ray.Advance(nearestIntersection);
			}
		}

		public static List<Vector3> GetAABBIntersectionPoints(Ray ray, AABB aabb)
		{

			Vector3 segmentBegin = ray.position;
			Vector3 segmentEnd = ray.position + ray.Direction * ray.maxDistance;
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
