using System;
using System.Drawing;
using System.Numerics;

namespace Raytracer
{
	public static class Extensions
	{

		public static float Fix(this float f, int dec)
		{
			int mul = 10 ^ dec;
			return (float)(Math.Round(f * mul) / mul);
		}

		public static bool Range(this float f, float min, float max)
		{
			return f >= min && f <= max;
		}

		public static float Clamp01(this float f)
		{
			return Math.Min(1, Math.Max(0, f));
		}

		public static float GetAxisValue(this Vector3 v, int axis)
		{
			if(axis == 0)
			{
				return v.X;
			}
			else if(axis == 1)
			{
				return v.Y;
			}
			else if(axis == 2)
			{
				return v.Z;
			}
			else
			{
				throw new IndexOutOfRangeException();
			}
		}

		public static Vector3 SetAxisValue(this Vector3 v, int axis, float value)
		{
			if(axis == 0)
			{
				v.X = value;
			}
			else if(axis == 1)
			{
				v.Y = value;
			}
			else if(axis == 2)
			{
				v.Z = value;
			}
			else
			{
				throw new IndexOutOfRangeException();
			}
			return v;
		}

		public static Vector2 XY(this Vector3 v)
		{
			return new Vector2(v.X, v.Y);
		}

		public static Vector2 XZ(this Vector3 v)
		{
			return new Vector2(v.X, v.Z);
		}

		public static Vector2 ZY(this Vector3 v)
		{
			return new Vector2(v.Z, v.Y);
		}

		public static bool IsInBounds(this Vector2 v)
		{
			return Math.Abs(v.X) <= 1 && Math.Abs(v.Y) <= 1;
		}

		public static Vector3 Abs(this Vector3 v)
		{
			return new Vector3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
		}

		public static Point ToPixelSpace(this Vector3 v, int w, int h)
		{
			v.X++;
			v.Y++;
			v.X /= 2f;
			v.Y /= 2f;
			int px = (int)Math.Round(v.X * w);
			int py = (int)Math.Round(v.Y * h);
			return new Point(px, py);
		}

		public static Point Trim(this Point p)
		{
			return new Point(
				Math.Min(Math.Max(p.X, -2000), 2000),
				Math.Min(Math.Max(p.Y, -2000), 2000)
			);
		}

		public static float Magnitude(this Vector2 v)
		{
			return Math.Abs(v.X) * Math.Abs(v.Y);
		}

		public static float DegToRad(this float f)
		{
			return (float)(Math.PI / 180f) * f;
		}

		public static float Lerp(float a, float b, float t)
		{
			return a * (1f - t) + b * t;
		}
	} 
}
