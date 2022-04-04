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

		private void label2_Click(object sender, EventArgs e) {

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e) {

		}

		private void splitter1_SplitterMoved(object sender, SplitterEventArgs e) {

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

		}

		private void OnRenderToFileMenuItemClick(object sender, EventArgs e)
		{

		}

		private void OnOpenSampleSceneMenuItemClick(object sender, EventArgs e)
		{

		}

		private void OnSaveSceneAsMenuItemClick(object sender, EventArgs e)
		{
			SceneFileWriter.SaveSceneAsPrompt();
		}
	}
}
