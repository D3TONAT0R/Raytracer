using ConsoleGameEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer {
	public class RaytracedRenderer {

		public static string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Raytracer");

		public static RaytracedRenderer instance;

		public static Action SceneLoaded;

		public string name => "Raytracer";

		bool render = false;
		bool maxRender = false;

		static Scene scene;
		public static Scene Scene {
			get {
				return scene;
			}
			set {
				scene = value;
				redrawScreen = true;
				SceneLoaded();
			}
		}
		public Camera camera;
		bool flyMode = true;
		public static bool redrawScreen = true;
		public static int stripsRendered = 0;

		static float movementSpeedScale = 1;

		public static RaytracerForm infoWindow;

		RenderSettings previewRenderSettings;
		RenderSettings hqRenderSettings;
		RenderSettings maxRenderSettings;

		public static RenderSettings CurrentSettings => instance.maxRender ? instance.maxRenderSettings : instance.render ? instance.hqRenderSettings : instance.previewRenderSettings;

		bool exit = false;
		bool animating = false;
		Thread loopthread;

		public void Run() {
			exit = false;
			hqRenderSettings = new RenderSettings(512, 288) {
				rayMarchDistanceInVoid = 0.1f,
				rayMarchDistanceInObject = 0.01f,
				rayDistanceDegradation = 0f,
				maxBounces = 2,
				lightingType = LightingType.RaytracedHardShadows
			};
			previewRenderSettings = new RenderSettings(192, 108) {
				rayMarchDistanceInVoid = 0.5f,
				rayMarchDistanceInObject = 0.1f,
				rayDistanceDegradation = 0.05f,
				maxBounces = 0,
				lightingType = LightingType.RaytracedNoShadows,
				allowSelfShadowing = false,
				specularHighlights = false
			};
			maxRenderSettings = new RenderSettings(1280, 720) {
				rayMarchDistanceInVoid = 0.1f,
				rayMarchDistanceInObject = 0.01f,
				rayDistanceDegradation = 0f,
				maxBounces = 4,
				lightingType = LightingType.RaytracedHardShadows
			};
			instance = this;
			int sceneIndex = 0;
			redrawScreen = true;
			MakeWinforms();
			camera = new Camera() {
				localPosition = Vector3.UnitY,
				rotation = Vector3.Zero,
				fieldOfView = 60
			};
			Scene = SceneBuilder.Generate(sceneIndex);
			var ts = new ThreadStart(LoopThread);
			loopthread = new Thread(ts);
			loopthread.SetApartmentState(ApartmentState.STA);
			loopthread.Start();
			while(!exit) {
				Thread.Sleep(250);
				//DrawInfoWindow();
				if(redrawScreen) DrawScreenOnWinform();
			}
		}

		void DrawScreenOnWinform() {
			redrawScreen = false;
			/*if(winform == null) return;
			graphics = winform.CreateGraphics();
			graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;*/
			if(animating) {
				animating = Animator.Animate();
			}
			camera.Render(Scene, CurrentSettings.renderBuffer);
			//graphics.DrawImage(CurrentSettings.renderBuffer, 0, 0, winform.Width, winform.Height);
			var str = $"Pos {camera.localPosition}\nRot {camera.rotation}\nRot dir {MathUtils.EulerToDir(camera.rotation)}\nFOV {camera.fieldOfView}\nFullrender: {render}";
			//graphics.DrawString(str, SystemFonts.MessageBoxFont, new SolidBrush(System.Drawing.Color.DarkRed), new PointF(0, 0));
			if(maxRender) {
				SaveScreenshot();
				maxRender = false;
			}
			if(animating) {
				SaveScreenshot("anim");
				redrawScreen = true;
			}
			infoWindow.Invoke((Action)delegate {
				infoWindow.imageViewer.Image = CurrentSettings.renderBuffer;
			});
		}

		void MakeWinforms() {
			Application.EnableVisualStyles();
			infoWindow = new RaytracerForm();
			Thread t2 = new Thread(new ThreadStart(RunInfoWindowThread));
			t2.SetApartmentState(ApartmentState.STA);
			t2.Start();
		}

		void RunInfoWindowThread() {
			Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
			if(infoWindow != null) {
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += new DoWorkEventHandler(WindowUpdateWorker);
				worker.RunWorkerAsync();
				infoWindow.ShowDialog();
			}
		}

		void WindowUpdateWorker(object sender, EventArgs e) {
			Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
			Thread.Sleep(100);
			try {
				while(!exit && infoWindow != null) {
					if(infoWindow.IsInitialized) {
						infoWindow.Invoke((Action)delegate {
							StringBuilder sb = new StringBuilder();
							if(camera.rendering) {
								sb.AppendLine("Rendering...");
								sb.AppendLine($"{stripsRendered}/{CurrentSettings.screenHeight}");
								int percent = (int)Math.Round(stripsRendered / (float)CurrentSettings.screenHeight);
							} else {
								sb.AppendLine("Idle.");
							}
							infoWindow.progressInfo.Text = sb.ToString();
							infoWindow.progressBar.Maximum = CurrentSettings.screenHeight;
							infoWindow.progressBar.Value = camera.rendering ? stripsRendered : infoWindow.progressBar.Maximum;
							movementSpeedScale = infoWindow.cameraSpeedScale.Value / 10f;
							BuildSceneTree();
						});
					}
					Thread.Sleep(100);
				}
			} catch {
				exit = true;
			}
		}

		void BuildSceneTree() {
			if(Scene != null && Scene.sceneContent != null && Scene.hasContentUpdate) {
				infoWindow.sceneTree.Nodes.Clear();
				var node = new TreeNode("Scene");
				foreach(var o in Scene.sceneContent) {
					TraverseTree(o, node);
				}
				infoWindow.sceneTree.Nodes.Add(node);
				Scene.hasContentUpdate = false;
			}
		}

		void TraverseTree(SceneObject obj, TreeNode node) {
			TreeNode newnode;
			if(obj is Group g) {
				newnode = new TreeNode(obj.ToString());
				foreach(var o2 in g.children) {
					TraverseTree(o2, newnode);
				}
			} else if(obj is BooleanSolid b) {
				newnode = new TreeNode(obj.ToString());
				foreach(var o2 in b.solids) {
					TraverseTree(o2, newnode);
				}
			} else {
				newnode = new TreeNode(obj.ToString());
			}
			newnode.Tag = obj;
			node.Nodes.Add(newnode);
		}

		void LoopThread() {
			while(true) {
				Thread.Sleep(50);
				Update();
			}
		}

		public static void RedrawScreen(bool ignoreRenderStatus) {
			if(!ignoreRenderStatus && (instance.render || instance.maxRender)) {
				return;
			}
			redrawScreen = true;
		}

		void Update() {
			if(exit) return;
			bool hasFocus = Form.ActiveForm != null;
			if(!hasFocus) return;
			Input.Update();
			if(Input.esc.isDown) {
				exit = true;
			}
			if(Input.b.isDown) {
				if(Animator.Duration > 0) {
					animating = !animating;
					if(animating) redrawScreen = true;
					Animator.time = 0;
				}
			}
			if(animating) return;
			if(Input.w.isPressed) {
				KeyPress(0, 0, 1, false);
			}
			if(Input.s.isPressed) {
				KeyPress(0, 0, -1, false);
			}
			if(Input.a.isPressed) {
				KeyPress(-1, 0, 0, false);
			}
			if(Input.d.isPressed) {
				KeyPress(1, 0, 0, false);
			}
			if(Input.q.isPressed) {
				KeyPress(0, -1, 0, false);
			}
			if(Input.e.isPressed) {
				KeyPress(0, 1, 0, false);
			}
			if(Input.arrowUp.isPressed) {
				KeyPress(-1, 0, 0, true);
			}
			if(Input.arrowDown.isPressed) {
				KeyPress(1, 0, 0, true);
			}
			if(Input.arrowLeft.isPressed) {
				KeyPress(0, -1, 0, true);
			}
			if(Input.arrowRight.isPressed) {
				KeyPress(0, 1, 0, true);
			}
			if(Input.y.isPressed) {
				KeyPress(0, 0, -1, true);
			}
			if(Input.x.isPressed) {
				KeyPress(0, 0, 1, true);
			}
			if(Input.nLeft.isDown) {
				MoveRemoteObject(-1, 0);
			}
			if(Input.nRight.isDown) {
				MoveRemoteObject(1, 0);
			}
			if(Input.nDown.isDown) {
				MoveRemoteObject(0, -1);
			}
			if(Input.nUp.isDown) {
				MoveRemoteObject(0, 1);
			}
			if(Input.comma.isPressed) {
				redrawScreen = true;
				camera.fieldOfView += 5;
			}
			if(Input.period.isPressed) {
				redrawScreen = true;
				camera.fieldOfView -= 5;
			}
			if(Input.r.isDown) {
				redrawScreen = true;
				render = !render;
			}
			if(Input.p.isDown) {
				SaveScreenshot();
			}
			if(Input.space.isDown) {
				maxRender = true;
				redrawScreen = true;
			}
		}

		void KeyPress(int x, int y, int z, bool arrowKey) {
			redrawScreen = true;
			if(arrowKey) {
				float rmul = (float)Math.Tan((camera.fieldOfView / 2f).DegToRad()) * movementSpeedScale;
				camera.Rotate(new Vector3(x, y, z) * 10 * rmul);
			} else {
				camera.Move(new Vector3(x, y, z) * movementSpeedScale, flyMode);
			}
		}

		void MoveRemoteObject(int x, int z) {
			if(Scene.remoteControlledObject != null) {
				redrawScreen = true;
				Scene.remoteControlledObject.localPosition += new Vector3(x, 0, z);
			}
		}

		void SaveScreenshot(string prefix = "screenshot") {
			if(CurrentSettings.renderBuffer != null) {
				int num = 1;
				var path = Path.Combine(rootPath, "Screenshots", prefix + "_");
				while(File.Exists(path + num.ToString("D4") + ".png")) {
					num++;
				}
				CurrentSettings.renderBuffer.Save(path + num.ToString("D4") + ".png");
			}
		}

		public static Color TraceRay(Scene scene, Ray ray, Shape excludeShape = null) {
			if(ray.reflectionIteration >= CurrentSettings.maxBounces + 1) return Color.Black;
			Vector3? hit = TraceRay(scene, ref ray, out var intersection, excludeShape);
			if(hit != null) {
				return intersection.GetColorAt((Vector3)hit, ray);
			} else {
				//We didn't hit anything, render the sky instead
				return scene.SampleSkybox(ray.Direction);
			}
		}

		public static Vector3? TraceRay(Scene scene, ref Ray ray, out Shape intersectingShape, Shape excludeShape = null) {
			var shapes = scene.GetIntersectingShapes(ray);
			intersectingShape = null;
			//Ignore excluded shape
			if(excludeShape != null && shapes.Contains(excludeShape)) shapes.Remove(excludeShape);
			if(shapes.Count > 0) {
				OptimizeRay(ray, shapes);
				while(scene.IsInWorldBounds(ray.position)) {
					var intersecting = scene.GetAABBIntersectingShapes(ray.position, shapes);
					if(intersecting.Length == 0) {
						//No AABB collision detected
						if(!ray.Advance(CurrentSettings.rayMarchDistanceInVoid + ray.travelDistance * CurrentSettings.rayDistanceDegradation)) {
							return null;
						}
					} else {
						for(int i = 0; i < intersecting.Length; i++) {
							var localpos = ray.position - intersecting[i].HierarchyPositionOffset;
							if(intersecting[i].Intersects(localpos)) {
								//We are about to hit something
								intersectingShape = intersecting[i];
								return ray.position;
							}
						}
						//If Advance returns false, we have reached the ray's maximum distance without hitting any surface
						if(!ray.Advance(CurrentSettings.rayMarchDistanceInObject + ray.travelDistance * CurrentSettings.rayDistanceDegradation)) {
							return null;
						}
					}
				}
			}
			return null;
		}

		static void OptimizeRay(Ray ray, List<Shape> shapes) {
			//Jump directly to the first intersection point (skip marching in empty space)
			float nearestIntersection = float.MaxValue;
			float farthestIntersection = 0;
			for(int i = 0; i < shapes.Count; i++) {
				var intersections = GetAABBIntersectionPoints(ray, shapes[i].ExpandedAABB/*scene.shapeAABBs[shapes[i]][1]*/);
				if(intersections.Count > 0) {
					nearestIntersection = Math.Min(nearestIntersection, Vector3.Distance(ray.position, intersections[0]));
				}
				if(intersections.Count > 1) {
					farthestIntersection = Math.Max(farthestIntersection, Vector3.Distance(ray.position, intersections[1]));
				}
			}
			if(farthestIntersection > 0) {
				ray.maxDistance = farthestIntersection;
			}
			if(nearestIntersection < float.MaxValue) {
				ray.Advance(nearestIntersection);
			}
		}

		public static List<Vector3> GetAABBIntersectionPoints(Ray ray, AABB aabb) {

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
			for(int axis = 0; axis < 3; axis++) {
				if(beginToEnd.GetAxisValue(axis) == 0) // parallel
				{
					if(beginToMin.GetAxisValue(axis) > 0 || beginToMax.GetAxisValue(axis) < 0)
						return intersections; // segment is not between planes
				} else {
					var t1 = beginToMin.GetAxisValue(axis) / beginToEnd.GetAxisValue(axis);
					var t2 = beginToMax.GetAxisValue(axis) / beginToEnd.GetAxisValue(axis);
					var tMin = Math.Min(t1, t2);
					var tMax = Math.Max(t1, t2);
					if(tMin > tNear) tNear = tMin;
					if(tMax < tFar) tFar = tMax;
					if(tNear > tFar || tFar < 0) return intersections;

				}
			}
			if(tNear >= 0 && tNear <= 1) intersections.Add(segmentBegin + beginToEnd * tNear);
			if(tFar >= 0 && tFar <= 1) intersections.Add(segmentBegin + beginToEnd * tFar);
			return intersections;
		}
	}
}
