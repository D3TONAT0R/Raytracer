using SimplexNoise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	[ObjectIdentifier("VOLUME")]
	public class Volume : SceneObject
	{
		public enum VolumeType
		{
			Box,
			Sphere
		}

		[DataIdentifier("TYPE")]
		public VolumeType volumeType = VolumeType.Box;
		[DataIdentifier("SIZE")]
		public Vector3 size = Vector3.One;
		[DataIdentifier("DENSITY", 0.01f)]
		public float density = 0.1f;
		[DataIdentifier("COLOR")]
		public Color color = Color.White;
		[DataIdentifier("FADE")]
		public float fade = 0;

		public override void SetupForRendering()
		{
			
		}

		public override AABB ComputeLocalShapeBounds()
		{
			return new AABB(-size * 0.5f, size * 0.5f);
		}

		public float GetDensity(Vector3 worldPos)
		{
			var localPos = WorldToLocalPoint(worldPos);
			if(volumeType == VolumeType.Box)
			{
				if(Math.Abs(localPos.X) <= size.X * 0.5f && Math.Abs(localPos.Y) <= size.Y * 0.5f && Math.Abs(localPos.Z) <= size.Z * 0.5f)
				{
					float noiseScale = 0.5f;
					float noiseStrength = 1.5f;
					float noise = GetNoiseFractal(worldPos, noiseScale, noiseStrength, 3);
					float multiplier = 1f;
					if(fade > 0)
					{
						Vector3 normPos = (localPos / size).Abs();
						float normBorderDist = 0.5f - Math.Max(normPos.X, Math.Max(normPos.Y, normPos.Z));
						multiplier = MathUtils.Saturate(MathUtils.InverseLerp(0, fade, normBorderDist));
					}
					return density * multiplier * noise;
				}
			}
			else
			{
				var normPos = localPos / size;
				float normDist = normPos.Length();
				if(normDist < 0.5)
				{
					float noiseScale = 0.5f;
					float noiseStrength = 1.5f;
					float noise = GetNoiseFractal(worldPos, noiseScale, noiseStrength, 3);
					float multiplier = 1f;
					if(fade > 0)
					{
						multiplier = MathUtils.Saturate(MathUtils.InverseLerp(0.5f, 0.5f - fade, normDist));
					}
					return density * multiplier * noise;
				}
			}
			return 0;
		}

		public bool IntersectsRay(Ray ray)
		{
			return ray.GetAABBIntersectionPoints(WorldSpaceShapeBounds, Matrix4x4.Identity, Matrix4x4.Identity, out _, out _) > 0;
		}

		private float GetNoiseFractal(Vector3 pos, float scale, float strength, int octaves)
		{
			float influence = 1f;
			float finalNoise = 0;
			for(int i = 0; i < octaves; i++)
			{
				float n = GetNoise(pos, scale) - 0.5f;
				finalNoise += n * influence;
				scale *= 2f;
				influence *= 0.5f;
			}
			finalNoise += 0.5f;
			return (finalNoise - 0.5f) * strength + 0.5f;
		}

		private float GetNoise(Vector3 pos, float scale)
		{
			return Noise.CalcPixel3D((int)(pos.X * 1000), (int)(pos.Y * 1000), (int)(pos.Z * 1000), 0.001f * scale) / 255f;
		}
	}
}
