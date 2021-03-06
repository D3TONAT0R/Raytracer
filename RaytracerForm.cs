using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
			for(int i = 0; i <= 5; i++) {
				//sceneSelection.Items.Add("Scene #: " + i);
			}
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

		private void RaytracerForm_Load(object sender, EventArgs e) {

		}

		private void OnSceneTreeSelect(object sender, TreeViewEventArgs e) {
			SceneEditor.InspectedObject = sceneTree.SelectedNode.Tag as SceneObject;
		}

		private void propertiesPanel_Paint(object sender, PaintEventArgs e) {

		}

		private void OnFocusEnter(object sender, EventArgs e) {
			
		}

		private void OnKeyDown(object sender, KeyEventArgs e) {
			
		}

		private void OnOpenSceneMenuItemClick(object sender, EventArgs args)
		{
			SceneLoader.LoadSceneFilePrompt();
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
			RaytracerEngine.SaveScreenshot();
		}
	}
}
