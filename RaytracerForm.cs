using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
				sceneSelection.Items.Add("Scene #: " + i);
			}
			RaytracerEngine.SceneLoaded += OnSceneLoaded;
		}

		void OnSceneLoaded() {
			Invoke((Action)delegate {
				if(RaytracerEngine.Scene != null) {
					sceneSelection.SelectedIndex = RaytracerEngine.Scene.sceneIndex;
				} else {
					sceneSelection.SelectedIndex = 0;
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

		private void OnChange(object sender, EventArgs e) {
			var selection = sceneSelection.SelectedItem.ToString();
			var value = int.Parse(selection.Split(':')[1].Trim());
			RaytracerEngine.Scene = SceneBuilder.Generate(value);
		}

		private void OnFocusEnter(object sender, EventArgs e) {
			
		}

		private void OnKeyDown(object sender, KeyEventArgs e) {
			
		}

		private void OnDropdownClosed(object sender, EventArgs e) {
			splitContainer1.Select();
		}
	}
}
