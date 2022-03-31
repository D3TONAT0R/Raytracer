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
	public class RaytracerEngine {

		public static string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Raytracer");

		public static RaytracerEngine instance;

		public static Action SceneLoaded;

		public string name => "Raytracer";

		static bool render = false;
		static bool toScreenshot = false;

		public bool IsRendering => render;

		static Scene scene;
		public static Scene Scene {
			get {
				return scene;
			}
			set {
				scene = value;
				redrawScreen = true;
				SceneLoaded.Invoke();
			}
		}
		public Camera camera;
		bool flyMode = true;
		public static bool redrawScreen = true;

		static float movementSpeedScale = 1;

		public static RaytracerForm infoWindow;

		public static List<RenderSettings> renderSettings;

		public static List<RenderTarget> renderTargets;

		static RenderSettings previewRenderSettings;
		static RenderTarget previewRenderTarget;

		public static int currentRenderSettingsIndex = 0;
		public static int currentRenderTargetIndex = 0;

		public static RenderSettings CurrentRenderSettings => render ? renderSettings[currentRenderSettingsIndex] : previewRenderSettings;
		public static RenderTarget CurrentRenderTarget => render ? renderTargets[currentRenderTargetIndex] : previewRenderTarget;

		bool exit = false;
		bool animating = false;
		Thread loopthread;

		public void Run() {
			instance = this;
			exit = false;
			SetupSettingsAndTargets();

			int sceneIndex = 0;
			redrawScreen = true;
			MakeWinforms();
			camera = new Camera() {
				localPosition = Vector3.UnitY,
				rotation = Vector3.Zero,
				fieldOfView = 60
			};
			Scene = SceneLoader.GeneratePreset(sceneIndex);
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

		void SetupSettingsAndTargets()
		{
			previewRenderSettings = new RenderSettings("Preview")
			{
				rayMarchDistanceInVoid = 0.5f,
				rayMarchDistanceInObject = 0.1f,
				rayDistanceDegradation = 0.05f,
				maxBounces = 0,
				lightingType = LightingType.RaytracedNoShadows,
				allowSelfShadowing = false,
				specularHighlights = false
			};
			var hqRenderSettings = new RenderSettings("Normal")
			{
				rayMarchDistanceInVoid = 0.1f,
				rayMarchDistanceInObject = 0.01f,
				rayDistanceDegradation = 0f,
				maxBounces = 2,
				lightingType = LightingType.RaytracedHardShadows
			};
			var maxRenderSettings = new RenderSettings("High")
			{
				rayMarchDistanceInVoid = 0.1f,
				rayMarchDistanceInObject = 0.01f,
				rayDistanceDegradation = 0f,
				maxBounces = 3,
				lightingType = LightingType.RaytracedHardShadows
			};
			renderSettings = new List<RenderSettings>();
			renderSettings.Add(hqRenderSettings);
			renderSettings.Add(maxRenderSettings);

			previewRenderTarget = new RenderTarget("Preview", 240, 135);
			renderTargets = new List<RenderTarget>();
			renderTargets.Add(new RenderTarget("SD", 640, 360));
			renderTargets.Add(new RenderTarget("HD", 1280, 720));
			renderTargets.Add(new RenderTarget("FHD", 1920, 1080));
			renderTargets.Add(new RenderTarget("WQHD", 2560, 1440));
			renderTargets.Add(new RenderTarget("3.2K", 3200, 1800));
			renderTargets.Add(new RenderTarget("4K", 3840, 2160));
			renderTargets.Add(new RenderTarget("8K", 7680, 4320));
		}

		void DrawScreenOnWinform() {
			redrawScreen = false;
			/*if(winform == null) return;
			graphics = winform.CreateGraphics();
			graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;*/
			if(animating) {
				animating = Animator.Animate();
			}
			camera.Render(Scene, CurrentRenderTarget);
			//graphics.DrawImage(CurrentSettings.renderBuffer, 0, 0, winform.Width, winform.Height);
			var str = $"Pos {camera.localPosition}\nRot {camera.rotation}\nRot dir {MathUtils.EulerToDir(camera.rotation)}\nFOV {camera.fieldOfView}\nFullrender: {render}";
			//graphics.DrawString(str, SystemFonts.MessageBoxFont, new SolidBrush(System.Drawing.Color.DarkRed), new PointF(0, 0));
			if(toScreenshot) {
				SaveScreenshot();
				toScreenshot = false;
			}
			if(animating) {
				SaveScreenshot("anim");
				redrawScreen = true;
			}
			RefreshImageView();
		}

		public void RefreshImageView()
		{
			lock (SceneRenderer.bufferLock)
			{
				infoWindow.Invoke((Action)delegate
				{
					infoWindow.imageViewer.Image = CurrentRenderTarget.RenderBuffer;
				});
			}
		}

		void MakeWinforms() {
			Application.EnableVisualStyles();
			Thread t2 = new Thread(new ThreadStart(RunInfoWindowThread));
			t2.SetApartmentState(ApartmentState.STA);
			t2.Start();
		}

		void RunInfoWindowThread() {
			//Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
			infoWindow = new RaytracerForm();
			SetupMenuStrip();
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler(WindowUpdateWorker);
			worker.RunWorkerAsync();
			infoWindow.ShowDialog();
		}

		void SetupMenuStrip()
		{
			
			var qList = infoWindow.qualityMenuItem.DropDownItems;
			qList.Clear();
			for (int i = 0; i < renderSettings.Count; i++)
			{
				var item = new ToolStripMenuItem
				{
					Name = "q" + i,
					Text = renderSettings[i].name,
					Tag = i
				};
				item.Click += OnRenderSettingsMenuItemClick;
				qList.Add(item);
			}

			var tList = infoWindow.resolutionMenuItem.DropDownItems;
			tList.Clear();
			for (int i = 0; i < renderTargets.Count; i++)
			{
				var rt = renderTargets[i];
				string text = $"{rt.name} ({rt.width}x{rt.height} - {(rt.PixelCount / 1000000f):F1} MP)";
				var item = new ToolStripMenuItem
				{
					Name = "r" + i,
					Text = text,
					Tag = i
				};
				item.Click += OnResolutionMenuItemClick;
				tList.Add(item);
			}
		}

		void UpdateRenderMenuItems()
		{
			infoWindow.Invoke((Action)delegate
			{
				for (int i = 0; i < renderSettings.Count; i++)
				{
					var item = infoWindow.qualityMenuItem.DropDownItems[i] as ToolStripMenuItem;
					item.Checked = i == currentRenderSettingsIndex;
				}
				for (int i = 0; i < renderTargets.Count; i++)
				{
					var item = infoWindow.resolutionMenuItem.DropDownItems[i] as ToolStripMenuItem;
					item.Checked = i == currentRenderTargetIndex;
				}
			});
		}

		private void OnRenderSettingsMenuItemClick(object sender, EventArgs e)
		{
			if(sender is ToolStripMenuItem mi)
			{
				SetRenderSettings((int)mi.Tag);
			}
			else
			{
				MessageBox.Show("wtf 1");
			}
		}

		private void OnResolutionMenuItemClick(object sender, EventArgs e)
		{
			if(sender is ToolStripMenuItem mi)
			{
				SetRenderResolution((int)mi.Tag);
			}
			else
			{
				MessageBox.Show("wtf 2");
			}
		}

		public void SetRenderSettings(int settingsIndex)
		{
			currentRenderSettingsIndex = settingsIndex;
			UpdateRenderMenuItems();
		}

		public void SetRenderResolution(int resolutionIndex)
		{
			currentRenderTargetIndex = resolutionIndex;
			UpdateRenderMenuItems();
		}

		void WindowUpdateWorker(object sender, EventArgs e) {
			//Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
			Thread.Sleep(100);
			try {
				UpdateRenderMenuItems();
				while(!exit && infoWindow != null) {
					if(infoWindow.IsInitialized) {
						infoWindow.Invoke((Action)delegate {
							StringBuilder sb = new StringBuilder();
							float progress = 1;
							if(camera.rendering) {
								sb.AppendLine("Rendering...");
								SceneRenderer.ActiveScreenRenderer.GetProgressInfo(out string progressString, out progress);
								sb.AppendLine(progressString);
							} else {
								sb.AppendLine("Idle.");
							}
							progress = float.IsNaN(progress) ? 0 : Math.Min(1, Math.Max(0, progress));
							infoWindow.progressInfo.Text = sb.ToString();
							infoWindow.progressBar.Maximum = 100;
							infoWindow.progressBar.Value = (int)(progress*100);
							movementSpeedScale = infoWindow.cameraSpeedScale.Value / 10f;
							BuildSceneTree();
						});
						if(SceneRenderer.IsRendering && IsRendering)
						{
							SceneRenderer.RequestImageRefresh();
							RefreshImageView();
						}
					}
					Thread.Sleep(SceneRenderer.IsRendering ? 500 : 125);
				}
			}
			catch
			{
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
			while(!exit) {
				Thread.Sleep(50);
				Update();
			}
		}

		public static void RedrawScreen(bool ignoreRenderStatus) {
			if(!ignoreRenderStatus && render) {
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
			var buffer = CurrentRenderTarget.RenderBuffer;
			if(buffer != null) {
				int num = 1;
				var path = Path.Combine(rootPath, "Screenshots", prefix + "_");
				while(File.Exists(path + num.ToString("D4") + ".png")) {
					num++;
				}
				buffer.Save(path + num.ToString("D4") + ".png");
			}
		}
	}
}
