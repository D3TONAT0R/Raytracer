using System;
using System.Numerics;

namespace Raytracer {
	public static class MathUtils {

		public const float Deg2Rad = (float)Math.PI / 180f;
		public const float Rad2Deg = 180f / (float)Math.PI;

		public static Vector3 Refract(Vector3 rayNrm, Vector3 surfaceNrm, float iorFrom, float iorTo) {
			if(iorFrom == iorTo) return rayNrm;
			//float r = -ior * (1f - Math.Abs(Vector3.Dot(rayNrm, surfaceNrm)));
			//return Vector3.Normalize(Vector3.Lerp(rayNrm, -surfaceNrm, -ior));
			float r = iorFrom / iorTo;
			var c = Vector3.Dot(-surfaceNrm, rayNrm);
			return r * rayNrm + (float)(r * c - Math.Sqrt(1f - Math.Pow(r, 2) * Math.Pow(c, 2))) * surfaceNrm;
		}

		public static float Step(float x, float a) {
			return (x - 0.5f + a) / a - 0.5f;
		}

		public static Vector3 Bounce(Vector3 dir, Vector3 surf) {
			var dot = Vector3.Dot(dir, surf);
			return dir - (2 * dot) * surf;
		}

		public static float Lerp(float a, float b, float t) {
			return a + (b - a) * t;
		}

		public static float Remap(float a, float b, float t) {
			return (t - a) / (b - a);
		}

		public static Vector2 RotateAround(Vector2 origin, float angle, Vector2 point) {
			angle *= Deg2Rad;
			float s = (float)Math.Sin(angle);
			float c = (float)Math.Cos(angle);
			point.X -= origin.X;
			point.Y -= origin.Y;
			float xnew = point.X * c - point.Y * s;
			float ynew = point.X * s + point.Y * c;
			point.X = xnew + origin.X;
			point.Y = ynew + origin.Y;
			return point;
		}

		public static float DegSin(float f) {
			return (float)Math.Sin(f * Deg2Rad);
		}

		public static float DegCos(float f) {
			return (float)Math.Cos(f * Deg2Rad);
		}

		public static float DegTan(float f) {
			return (float)Math.Tan(f * Deg2Rad);
		}

		public static Vector3 EulerToDir(Vector3 euler) {
			float pitch = Deg2Rad * -euler.X;
			float yaw = Deg2Rad * euler.Y;

			var dy = (float)Math.Sin(pitch);
			var dx = (float)Math.Sin(yaw) * (float)Math.Cos(pitch);
			var dz = (float)Math.Cos(yaw) * (float)Math.Cos(pitch);

			return new Vector3(dx, dy, dz);
		}

		public static Vector3 DirToEuler(Vector3 dir) {
			dir = Vector3.Normalize(dir);
			if(dir.Y == 1) {
				return new Vector3(-90, 0, 0);
			} else if(dir.Y == -1) {
				return new Vector3(90, 0, 0);
			}
			float yaw = (float)Math.Atan(dir.X / dir.Z) * Rad2Deg;
			if(dir.Z <= 0) yaw += 180;
			float pitch;
			if(dir.Y != 0) {
				pitch = (float)Math.Atan(Math.Sqrt(Math.Pow(dir.X, 2) + Math.Pow(dir.Z, 2)) / dir.Y) * Rad2Deg;
				if(dir.Y <= 0) pitch += 180;
			} else {
				pitch = 90;
			}
			return new Vector3(pitch - 90, yaw, 0);
		}

		public static float Dir2DToAngle01(float x, float y)
		{
			var v2 = Vector2.Normalize(new Vector2(x, y));
			float a = (float)Math.Sinh(v2.X) * 76.582709f;
			a /= 360f;
			if(v2.Y < 0)
			{
				a = 0.5f - a;
			}
			return (a + 1.0f) % 1.0f;
		}
	}
}
