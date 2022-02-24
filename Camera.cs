﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Raytracer.RaytracerEngine;
using static Raytracer.CurrentPixelRenderData;

namespace Raytracer {

	[ObjectIdentifier("CAMERA")]
	public class Camera : SceneObject {

		[DataIdentifier("ROTATION")]
		public Vector3 rotation;
		[DataIdentifier("FOV")]
		public float fieldOfView = 60;

		private Matrix4x4 cameraMatrix;
		private float aspectRatio;
		public Vector3 cornerDir;

		public void Move(Vector3 localMoveVector, bool fly) {
			var euler = rotation * MathUtils.Deg2Rad;
			if(fly) {
				euler.X = 0;
				euler.Z = 0;
			}
			cameraMatrix = Matrix4x4.CreateFromYawPitchRoll(euler.Y, euler.X, euler.Z);
			var fwd = Forward(cameraMatrix);
			var right = Right(cameraMatrix);
			var up = Up(cameraMatrix);
			localPosition += fwd * localMoveVector.Z;
			localPosition += right * localMoveVector.X;
			localPosition += up * localMoveVector.Y;
		}

		public void Rotate(Vector3 rot) {
			rotation += rot;
		}

		public void SetScreenParams(int w, int h) {
			screenWidth = w;
			screenHeight = h;
			aspectRatio = h / (float)w;
			var cd = new Vector3(MathUtils.DegTan(fieldOfView/2f) / aspectRatio, MathUtils.DegTan(-fieldOfView/2f), 1);
			cornerDir = cd;
			var euler = rotation * MathUtils.Deg2Rad;
			cameraMatrix = Matrix4x4.CreateFromYawPitchRoll(euler.Y, euler.X, euler.Z);
		}

		public Ray ScreenPointToRay(Vector2 viewport) {
			var offsetEuler = MathUtils.DirToEuler(new Vector3(viewport, 1) * cornerDir) * MathUtils.Deg2Rad;
			var offset = Matrix4x4.CreateFromYawPitchRoll(offsetEuler.Y, -offsetEuler.X, 0);
			var result = offset * cameraMatrix;
			var dir = Forward(result);
			return new Ray(localPosition, dir, 0, Vector2.Zero);
		}

		Vector3 Forward(Matrix4x4 m) {
			return Vector3.Normalize(new Vector3(m.M31, m.M32, m.M33));
		}

		Vector3 Right(Matrix4x4 m) {
			return Vector3.Normalize(new Vector3(m.M11, m.M12, m.M13));
		}

		Vector3 Up(Matrix4x4 m) {
			return Vector3.Normalize(new Vector3(m.M21, m.M22, m.M23));
		}

		private readonly object locker = new object();
		public bool rendering;

		public void Render(Scene scene, Bitmap buffer) {
			if(rendering) return;
			rendering = true;
			scene.OnBeginRender(CurrentSettings.rayMarchDistanceInVoid);
			SetScreenParams(buffer.Width, buffer.Height);
			lock(locker)
			{
				SceneRenderer.RenderScene(this, scene, buffer);
			}
			rendering = false;
		}

		private float Lerp(float a, float b, float t) {
			return a + (b - a) * t;
		}

		private Vector3 Lerp(Vector3 a, Vector3 b, float t) {
			Vector3 v = new Vector3 {
				X = Lerp(a.X, b.X, t),
				Y = Lerp(a.Y, b.Y, t),
				Z = Lerp(a.Z, b.Z, t)
			};
			return v;
		}
	}
}
