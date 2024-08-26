using System;
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

		public static Camera MainCamera { get; set; }

		[DataIdentifier("FOV")]
		public float fieldOfView = 60;
		[DataIdentifier("OFFSET")]
		public float forwardOffset = 0;

		private Matrix4x4 cameraMatrix;
		private float aspectRatio;
		private Vector3 cornerDir;

		public Vector3 ActualCameraPosition => WorldPosition + MathUtils.EulerToDir(localRotation) * forwardOffset;

		public event Action HasChanged;

		public void ApplyConfiguration(CameraConfiguration configuration)
		{
			localPosition = configuration.position;
			localRotation = configuration.rotation;
			fieldOfView = configuration.fieldOfView;
			forwardOffset = configuration.offset;
			HasChanged?.Invoke();
		}

		public void Move(Vector3 localMoveVector, bool fly) {
			var translationEuler = localRotation * MathUtils.Deg2Rad;
			if(fly) {
				translationEuler.X = 0;
				translationEuler.Z = 0;
			}
			var translationMatrix = Matrix4x4.CreateFromYawPitchRoll(translationEuler.Y, translationEuler.X, translationEuler.Z);
			var fwd = Forward(translationMatrix);
			var right = Right(translationMatrix);
			var up = Up(translationMatrix);
			localPosition += fwd * localMoveVector.Z;
			localPosition += right * localMoveVector.X;
			localPosition += up * localMoveVector.Y;
			HasChanged?.Invoke();
		}

		public void MoveOffset(float offsetAdd)
		{
			forwardOffset += offsetAdd;
			HasChanged?.Invoke();
		}

		public void Rotate(Vector3 rot) {
			localRotation += rot;
			HasChanged?.Invoke();
		}

		public void SetScreenParams(int w, int h) {
			screenWidth = w;
			screenHeight = h;
			aspectRatio = h / (float)w;
			var cd = new Vector3(MathUtils.DegTan(fieldOfView/2f) / aspectRatio, MathUtils.DegTan(-fieldOfView/2f), 1);
			cornerDir = cd;
			var euler = localRotation * MathUtils.Deg2Rad;
			cameraMatrix = Matrix4x4.CreateFromYawPitchRoll(euler.Y, euler.X, euler.Z);
		}

		public Ray ScreenPointToRay(Vector2 viewport) {
			var offsetEuler = MathUtils.DirToEuler(new Vector3(viewport, 1) * cornerDir) * MathUtils.Deg2Rad;
			var offset = Matrix4x4.CreateFromYawPitchRoll(offsetEuler.Y, -offsetEuler.X, 0);
			var result = offset * cameraMatrix;
			var dir = Forward(result);
			return new Ray(ActualCameraPosition, dir, 0, Vector2.Zero);
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

		public void Render(Scene scene, RenderTarget target) {
			if(rendering) return;
			rendering = true;
			if (scene != null)
			{
				scene.OnBeginRender();
			}
			SetScreenParams(target.width, target.height);
			lock(locker)
			{
				if (scene != null)
				{
					SceneRenderer.RenderScene(this, scene, target.RenderBuffer);
				}
				else
				{
					//Draw empty screen
				}
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

		public override void SetupForRendering()
		{
			
		}
	}
}
