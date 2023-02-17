using Raytracer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer {
	public class RaytracerEngine {

		public static string rootPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Raytracer");

		public static RaytracerEngine instance;

		public static Action SceneLoaded;

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
				UpdateCameraConfigurationItems();
				SceneLoaded?.Invoke();
			}
		}
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

		public static string ScreenshotDirectory => Path.Combine(rootPath, "Screenshots");

		static bool exit = false;
		static bool animating = false;
		Thread loopthread;

		static Stopwatch renderTimer;
		static RenderTarget lastRenderTarget;

		public static void Main(string[] args)
		{
			var engine = new RaytracerEngine();
			engine.Start();
		}

		public void Start() {
			instance = this;
			exit = false;
			SetupSettingsAndTargets();

			redrawScreen = true;
			MakeWinforms();
			Camera.MainCamera = new Camera() {
				localPosition = Vector3.UnitY,
				rotation = Vector3.Zero,
				fieldOfView = 60
			};
			Camera.MainCamera.HasChanged += PersistentPrefs.WriteLastSessionInfo;
			var ts = new ThreadStart(LoopThread);
			loopthread = new Thread(ts);
			loopthread.SetApartmentState(ApartmentState.STA);
			loopthread.Start();
			while(!exit) {
				Thread.Sleep(250);
				if (infoWindow == null || !infoWindow.Visible) exit = true;
				//DrawInfoWindow();
				if(redrawScreen) DrawScreenOnWinform();
			}
		}

		void SetupSettingsAndTargets()
		{
			previewRenderSettings = new RenderSettings("Preview")
			{
				rayMarchDistanceInVoid = 0.5f,
				rayMarchDistanceInObject = 0.03f,
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
				lightingType = LightingType.RaytracedHardShadows,
			};
			var maxRenderSettings = new RenderSettings("High")
			{
				rayMarchDistanceInVoid = 0.03f,
				rayMarchDistanceInObject = 0.002f,
				rayDistanceDegradation = 0f,
				maxBounces = 5,
				lightingType = LightingType.RaytracedHardShadows
			};
			renderSettings = new List<RenderSettings>();
			renderSettings.Add(hqRenderSettings);
			renderSettings.Add(maxRenderSettings);

			previewRenderTarget = new RenderTarget("Preview", 240, 135);
			renderTargets = new List<RenderTarget>();
			renderTargets.Add(new RenderTarget("LD", 480, 270));
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

			if (render)
			{
				if (renderTimer == null) renderTimer = new Stopwatch();
				lastRenderTarget = CurrentRenderTarget;

				renderTimer.Restart();
			}

			Camera.MainCamera.Render(Scene, CurrentRenderTarget);

			if (render && renderTimer != null) renderTimer.Stop();

			if(toScreenshot) {
				SaveScreenshot();
				toScreenshot = false;
			}

			if (animating)
			{
				SaveScreenshot("anim");
				redrawScreen = true;
			}

			if(animating && Scene.animator != null)
			{
				Scene.animator.StepFrames(1);
				animating = Scene.animator.HasNextFrame;
			}

			RefreshImageView();

			if (render && !animating && renderTimer.ElapsedMilliseconds > 10000)
			{
				MessageBox.Show($"Time elapsed: {renderTimer.Elapsed:mm\\:ss}\nResolution: {lastRenderTarget}\nRender Settings: {CurrentRenderSettings.name}", "Render Finished", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
			}

			if(!animating) render = false;
		}

		public void RefreshImageView()
		{
			if (infoWindow == null) return;
			lock (SceneRenderer.bufferLock)
			{
				try
				{
					infoWindow.Invoke((Action)delegate
					{
						infoWindow.imageViewer.Image = CurrentRenderTarget.RenderBuffer;
					});
				}
				catch
				{

				}
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

		public static void UpdateCameraConfigurationItems()
		{
			infoWindow.Invoke((Action)delegate
			{
				var items = new List<ToolStripItem>();
				int i = 0;
				infoWindow.cameraMenuItem.DropDownItems.Clear();
				foreach(var c in scene.cameraConfigurations)
				{
					string name = $"{i} - {c.name ?? "UNNAMED"}";
					var menuItem = new ToolStripMenuItem(name);
					int i1 = i;
					menuItem.Click += (object sender, EventArgs e) => OnCameraConfigurationItemClick(i1);
					items.Add(menuItem);
					i++;
				}
				items.Add(new ToolStripSeparator());
				var getCurrentMenuItem = new ToolStripMenuItem("Current configuration info ...");
				getCurrentMenuItem.Click += (object sender, EventArgs e) => GetCurrentCameraConfiguration();
				items.Add(getCurrentMenuItem);
				infoWindow.cameraMenuItem.DropDownItems.AddRange(items.ToArray());
			});
		}

		private static void OnCameraConfigurationItemClick(int index)
		{
			redrawScreen = true;
			Camera.MainCamera.ApplyConfiguration(scene.cameraConfigurations[index]);
		}

		private static void GetCurrentCameraConfiguration()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Current camera configuration:");
			var cam = Camera.MainCamera;
			sb.AppendLine("Position: " + cam.localPosition.ToString());
			sb.AppendLine("Rotation: " + cam.rotation.ToString());
			sb.AppendLine("FOV: " + cam.fieldOfView.ToString());
			MessageBox.Show(sb.ToString());
		}

		private void OnRenderSettingsMenuItemClick(object sender, EventArgs e)
		{
			if(sender is ToolStripMenuItem mi)
			{
				SetRenderSettings((int)mi.Tag);
			}
		}

		private void OnResolutionMenuItemClick(object sender, EventArgs e)
		{
			if(sender is ToolStripMenuItem mi)
			{
				SetRenderResolution((int)mi.Tag);
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

		public static void BeginRender(bool directlyToFile)
		{
			if (render) return;
			render = true;
			redrawScreen = true;
			toScreenshot = directlyToFile;
		}

		public static void CancelRender()
		{
			render = false;
			animating = false;
			renderTimer.Reset();
			redrawScreen = true;
		}

		public static void QuitApplication()
		{
			exit = true;
		}

		void WindowUpdateWorker(object sender, EventArgs e) {
			//Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
			Thread.Sleep(500);
			try {
				UpdateRenderMenuItems();
				while(!exit && infoWindow != null) {
					if(infoWindow.IsInitialized) {
						infoWindow.Invoke((Action)delegate {
							StringBuilder sb = new StringBuilder();
							float progress = 1;
							if(render && Camera.MainCamera.rendering) {
								string renderText = "Rendering ";
								if(animating) renderText += $"({scene.animator.CurrentFrame}/{scene.animator.TotalRecFrameCount})";
								renderText += "...";
								sb.AppendLine(renderText);
								SceneRenderer.ActiveScreenRenderer.GetProgressInfo(out string progressString, out progress);
								sb.AppendLine(progressString);
								if (render && lastRenderTarget != null)
								{
									sb.AppendLine(lastRenderTarget.ToString());
									sb.Append($"Time: {renderTimer.Elapsed:mm\\:ss}");
								}
							} else {
								sb.AppendLine("Idle.");
								if (lastRenderTarget != null)
								{
									sb.AppendLine($"Last Render: {renderTimer.Elapsed:mm\\:ss}");
									sb.Append("  @ " + lastRenderTarget.ToString());
								}
							}
							progress = float.IsNaN(progress) ? 0 : Math.Min(1, Math.Max(0, progress));
							infoWindow.progressInfo.Text = sb.ToString();
							infoWindow.progressBar.Maximum = 100;
							infoWindow.progressBar.Value = (int)(progress*100);
							movementSpeedScale = infoWindow.cameraSpeedScale.Value / 20f;
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
				if (b.solids != null)
				{
					foreach (var o2 in b.solids)
					{
						TraverseTree(o2, newnode);
					}
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
			if(!AppHasFocus() || Form.ActiveForm != infoWindow) return;
			Input.Update();
			if(Input.esc.isDown) {
				exit = true;
			}
			if(Input.b.isDown) {
				if(Scene.animator.RecDuration > 0) {
					animating = !animating;
					render = animating;
					if(animating)
					{
						redrawScreen = true;
						Scene.animator.Rewind();
					}
				}
			}
			if(animating || render) return;
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
			if(Input.n.isPressed)
			{
				Camera.MainCamera.MoveOffset(-1f * movementSpeedScale);
				redrawScreen = true;
			}
			if(Input.m.isPressed)
			{
				Camera.MainCamera.MoveOffset(1f * movementSpeedScale);
				redrawScreen = true;
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
				Camera.MainCamera.fieldOfView += 5;
			}
			if(Input.period.isPressed) {
				redrawScreen = true;
				Camera.MainCamera.fieldOfView -= 5;
			}

			if(scene != null && scene.animator != null)
			{
				if(Input.o.isPressed)
				{
					scene.animator.StepFrames(-1);
					redrawScreen = true;
				}
				if(Input.p.isPressed)
				{
					scene.animator.StepFrames(1);
					redrawScreen = true;
				}
				if(Input.l.isPressed)
				{
					scene.animator.Rewind();
					redrawScreen = true;
				}
			}
		}

		void KeyPress(int x, int y, int z, bool rotate) {
			redrawScreen = true;
			if(rotate) {
				float rmul = (float)Math.Tan((Camera.MainCamera.fieldOfView / 2f).DegToRad()) * movementSpeedScale;
				Camera.MainCamera.Rotate(new Vector3(x, y, z) * 10 * rmul);
			} else {
				Camera.MainCamera.Move(new Vector3(x, y, z) * movementSpeedScale, flyMode);
			}
		}

		void MoveRemoteObject(int x, int z) {
			if(Scene.remoteControlledObject != null) {
				redrawScreen = true;
				Scene.remoteControlledObject.localPosition += new Vector3(x, 0, z);
			}
		}

		public static void SaveScreenshot(string prefix = "screenshot") {
			var buffer = (lastRenderTarget ?? CurrentRenderTarget).RenderBuffer;
			if(buffer != null) {
				int num = 1;
				var path = Path.Combine(ScreenshotDirectory, prefix + "_");
				while(File.Exists(path + num.ToString("D4") + ".png")) {
					num++;
				}
				buffer.Save(path + num.ToString("D4") + ".png");
			}
		}

		/// <summary>Returns true if the current application has focus, false otherwise</summary>
		public static bool AppHasFocus()
		{
			var activatedHandle = GetForegroundWindow();
			if (activatedHandle == IntPtr.Zero)
			{
				return false;       // No window is currently activated
			}

			var procId = Process.GetCurrentProcess().Id;
			int activeProcId;
			GetWindowThreadProcessId(activatedHandle, out activeProcId);

			return activeProcId == procId;
		}


		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
	}
}
