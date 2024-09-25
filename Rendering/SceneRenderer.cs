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
		/// <summary>
		/// Gets or sets a value indicating whether progressive updates are enabled.
		/// </summary>
		public static bool progressiveUpdates = true;

		/// <summary>
		/// Gets the active screen renderer based on the current rendering settings.
		/// </summary>
		public static ScreenRenderer ActiveScreenRenderer
		{
			get
			{
				return progressiveRenderer;
				//return progressiveUpdates && RaytracerEngine.instance.IsRendering ? progressiveRenderer : stripRenderer;
			}
		}

		static ScreenRenderer stripRenderer = new FullscreenStripRenderer();
		static ScreenRenderer progressiveRenderer = new ProgressiveChunkRenderer();

		public static object bufferLock = new object();

		static Bitmap currentTarget;
		static BitmapData currentBitmapData;
		static byte[] currentByteBuffer;

		/// <summary>
		/// Gets a value indicating whether the scene is currently being rendered.
		/// </summary>
		public static bool IsRendering { get; private set; }

		/// <summary>
		/// Renders the scene to the specified target bitmap using the provided camera.
		/// </summary>
		/// <param name="camera">The camera used to render the scene.</param>
		/// <param name="scene">The scene to be rendered.</param>
		/// <param name="target">The target bitmap to render the scene onto.</param>
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
			lock(bufferLock)
			{
				Lock();
				Marshal.Copy(currentBitmapData.Scan0, currentByteBuffer, 0, currentByteBuffer.Length);
				Unlock();
			}
		}

		/// <summary>
		/// Flushes the current byte buffer to the target bitmap.
		/// </summary>
		public static void FlushCurrent()
		{
			lock(bufferLock)
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

		/// <summary>
		/// Requests a refresh of the image on the screen.
		/// </summary>
		public static void RequestImageRefresh()
		{
			FlushCurrent();
		}

		/// <summary>
		/// Traces a ray in the scene and returns the color of the intersecting object.
		/// </summary>
		/// <param name="scene">The scene to trace the ray in.</param>
		/// <param name="ray">The ray to trace.</param>
		/// <param name="rayType">The type of ray. Used in visibility checks.</param>
		/// <param name="excludeShape">An optional shape to exclude from the trace.</param>
		/// <param name="optimize">If true, enables optimization for faster tracing.</param>
		/// <returns>The rendered color from the traced ray.</returns>
		public static Color TraceRay(Scene scene, Ray ray, VisibilityFlags rayType, Shape excludeShape = null, bool optimize = true)
		{
			if(ray.reflectionIteration >= RaytracerEngine.CurrentRenderSettings.maxBounces + 1) return Color.Black;
			bool hit = TraceRay(scene, ref ray, rayType, out var result, excludeShape, null, optimize);

			if(hit && result.HitShape != null)
			{
				return result.HitShape.GetColorAt(result.HitShape.WorldToLocalPoint(result.Position), ray);
			}
			else
			{
				//We didn't hit anything, render the sky instead
				return scene.environment.SampleSkybox(ray.Direction);
			}
		}

		/// <summary>
		/// Traces a ray in the scene and returns the result of the intersection.
		/// </summary>
		/// <param name="scene">The scene to trace the ray in.</param>
		/// <param name="ray">The ray to trace.</param>
		/// <param name="rayType">The type of ray. Used in visibility checks.</param>
		/// <param name="result">Contains information on the tracing result.</param>
		/// <param name="excludeShape">An optional shape to exclude from the trace.</param>
		/// <param name="exitShape">Optional. If set, returns the position at which the ray has left the shape. The ray must start inside the object.</param>
		/// <param name="optimize">If true, enables optimization for faster tracing.</param>
		/// <returns>True, if the tracing returned a result.</returns>
		public static bool TraceRay(Scene scene, ref Ray ray, VisibilityFlags rayType, out RayTraceResult result, Shape excludeShape = null, Shape exitShape = null, bool optimize = true)
		{
			var shapes = scene.GetIntersectingShapes(ray, rayType);
			result = RayTraceResult.NoTrace;
			//Ignore excluded shape
			if(excludeShape != null && shapes.Contains(excludeShape)) shapes.Remove(excludeShape);
			if(shapes.Count > 0)
			{
				//TODO: optimization is temporarily disabled on reflections, causes floating reflections at intersections
				if(optimize) ray.AdvanceToNextShapeBounds(shapes);
				while(scene.IsInWorldBounds(ray.Position))
				{
					var intersecting = scene.GetAABBIntersectingShapes(ray.Position, shapes);
					if(intersecting.Length == 0)
					{
						//No AABB collision detected
						if (exitShape != null)
						{
							result = new RayTraceResult(null, ray.Position, ray.travelDistance);
							return true;
						}
						if(!ray.March(false))
						{
							//TODO: Not sure if this is really needed
							result = new RayTraceResult(null, ray.Position, ray.travelDistance);
							return true;
						}
					}
					else
					{
						for(int i = 0; i < intersecting.Length; i++)
						{
							var localPos = intersecting[i].WorldToLocalPoint(ray.Position);
							if(intersecting[i].Intersects(localPos))
							{
								//We are about to hit something
								result = new RayTraceResult(intersecting[i], ray.Position, ray.travelDistance);
								return true;
							}
						}

						var maxReached = !ray.March(true);
						//If Advance returns false, we have reached the ray's maximum distance without hitting any surface
						if(maxReached)
						{
							result = new RayTraceResult(null, ray.Position, ray.travelDistance);
							return false;
						}

						if(exitShape != null && exitShape.Intersects(exitShape.WorldToLocalPoint(ray.Position)))
						{
							//We are no longer in contact with the given shape, return the current position instead
							if(ray.travelDistance == 0) throw new InvalidOperationException("Not in contact with target shape after a distance of 0 units.");
							result = new RayTraceResult(null, ray.Position, ray.travelDistance);
							return true;
						}
					}
				}
			}
			else
			{
				ray.Advance(ray.maxDistance);
				result = new RayTraceResult(null, ray.Position, ray.travelDistance);
				return false;
			}
			return false;
		}
	}
}
