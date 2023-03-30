using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer {
	public partial class RaytracerForm : Form {

		public bool IsInitialized => this.IsHandleCreated;

		public RaytracerForm() {
			InitializeComponent();
			RaytracerEngine.SceneLoaded += OnSceneLoaded;
		}

		void OnSceneLoaded() {
			Invoke((Action)delegate {
				if(RaytracerEngine.Scene != null) {
					Text = $"Raytracer - {RaytracerEngine.Scene.sceneName}";
				} else {
					Text = $"Raytracer - (no scene loaded)";
				}
			});
		}

		public TreeNode FindNode(object tag)
		{
			foreach(TreeNode node in sceneTree.Nodes)
			{
				if(SearchRecursive(node, tag, out var result)) return result;
			}
			return null;
		}

		private bool SearchRecursive(TreeNode parent, object tag, out TreeNode result)
		{
			if(parent.Tag == tag)
			{
				result = parent;
				return true;
			}
			foreach(TreeNode child in parent.Nodes)
			{
				if(SearchRecursive(child, tag, out result)) return true;
			}
			result = null;
			return false;
		}

		private void RaytracerForm_Load(object sender, EventArgs e) {

		}

		private void OnSceneTreeSelect(object sender, TreeViewEventArgs e) {
			SceneEditor.InspectedObject = sceneTree.SelectedNode.Tag;
		}

		private void OnFocusEnter(object sender, EventArgs e) {
			
		}

		private void OnKeyDown(object sender, KeyEventArgs e) {
			
		}

		private void OnOpenSceneMenuItemClick(object sender, EventArgs args)
		{
			SceneGenerator.LoadSceneFilePrompt();
		}

		private void OnRenderMenuItemClick(object sender, EventArgs e)
		{
			RaytracerEngine.BeginRender(false);
		}

		private void OnRenderToFileMenuItemClick(object sender, EventArgs e)
		{
			RaytracerEngine.BeginRender(true);
		}

		private void OnOpenSampleSceneMenuItemClick(object sender, EventArgs e)
		{

		}

		private void OnSaveSceneAsMenuItemClick(object sender, EventArgs e)
		{
			SceneFileWriter.SaveSceneAsPrompt();
		}

		private void OnCancelRenderMenuItemClick(object sender, EventArgs e)
		{
			RaytracerEngine.CancelRender();
		}

		private void OnQuitMenuItemClick(object sender, EventArgs e)
		{
			RaytracerEngine.QuitApplication();
		}

		private void OnSaveViewMenuItemClick(object sender, EventArgs e)
		{
			ScreenshotExporter.SaveScreenshot();
		}

		private void reloadCurrentSceneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(RaytracerEngine.Scene != null)
			{
				SceneGenerator.ReloadCurrent();
			}
			else
			{
				try
				{
					if(PersistentPrefs.TryGetLastSessionInfo(out var info))
					{
						RaytracerEngine.Scene = SceneFileLoader.CreateFromFile(info.sceneFile);
						var cam = Camera.MainCamera;
						cam.localPosition = info.cameraPos;
						cam.localRotation = info.cameraRot;
						cam.fieldOfView = info.cameraFOV;
						cam.forwardOffset = info.cameraOffset;
					}
				}
				catch
				{
					MessageBox.Show("Failed to restore last session", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("W/S: Move Forward / Back");
			sb.AppendLine("A/D: Move Left / Right");
			sb.AppendLine("E/Q: Move UP / Down");
			sb.AppendLine("Arrow L/R: Look Left / Right");
			sb.AppendLine("Arrow Up/Down: Look Up / Down");
			sb.AppendLine("Y/X: Tilt Camera");
			sb.AppendLine("./,: Zoom");
			sb.AppendLine("N/M: Pivot Forward / Back");
			MessageBox.Show(sb.ToString(), "Camera Controls");
		}

		private void browseScreenshotFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("explorer.exe", ScreenshotExporter.ScreenshotRootFolder);
		}

		private void propertiesPanel_Layout(object sender, LayoutEventArgs e)
		{
			propertiesPanel.HorizontalScroll.Maximum = 0;
			propertiesPanel.AutoScroll = false;
			propertiesPanel.HorizontalScroll.Visible = false;
			propertiesPanel.AutoScroll = true;
		}
	}
}
