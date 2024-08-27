using System.Numerics;

namespace Raytracer
{
	public struct RayTraceResult
	{
		public readonly bool Hit;
		public readonly float TravelDistance;
		public readonly Vector3 Position;
		public readonly Shape HitShape;

		public static readonly RayTraceResult NoTrace = new RayTraceResult(null, Vector3.Zero, 0);

		public RayTraceResult(Shape hitShape, Vector3 position, float travelDistance)
		{
			Hit = hitShape != null;
			this.HitShape = hitShape;
			this.TravelDistance = travelDistance;
			this.Position = position;
		}
	}
}
