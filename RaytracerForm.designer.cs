
using Raytracer;
using System.Windows.Forms;
using System.Windows.Input;

namespace Raytracer {
	public partial class RaytracerForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if(!msg.HWnd.Equals(this.Handle) &&
				(keyData == Keys.Left || keyData == Keys.Right ||
				keyData == Keys.Up || keyData == Keys.Down))
				return true;
			return base.ProcessCmdKey(ref msg, keyData);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node2");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Node4");
			System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Node5");
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Node8");
			System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Node6", new System.Windows.Forms.TreeNode[] {
            treeNode6});
			System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Node3", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode7});
			System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Node7");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RaytracerForm));
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.progressInfo = new System.Windows.Forms.RichTextBox();
			this.button_maxrender = new System.Windows.Forms.Button();
			this.button_screenshot = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.maxBounceCount = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.cameraSpeedScale = new System.Windows.Forms.TrackBar();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.sceneTree = new System.Windows.Forms.TreeView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.propertiesPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.sceneSelection = new System.Windows.Forms.ComboBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
			this.bbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aaToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.vvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripSplitButton2 = new System.Windows.Forms.ToolStripSplitButton();
			this.imageViewer = new Raytracer.SceneViewerPictureBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.maxBounceCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cameraSpeedScale)).BeginInit();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageViewer)).BeginInit();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(3, 388);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(143, 23);
			this.progressBar.TabIndex = 1;
			// 
			// progressInfo
			// 
			this.progressInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.progressInfo.Location = new System.Drawing.Point(3, 341);
			this.progressInfo.Name = "progressInfo";
			this.progressInfo.ReadOnly = true;
			this.progressInfo.Size = new System.Drawing.Size(143, 41);
			this.progressInfo.TabIndex = 4;
			this.progressInfo.Text = "Render Info\n...\n...";
			// 
			// button_maxrender
			// 
			this.button_maxrender.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button_maxrender.Location = new System.Drawing.Point(3, 283);
			this.button_maxrender.Name = "button_maxrender";
			this.button_maxrender.Size = new System.Drawing.Size(143, 23);
			this.button_maxrender.TabIndex = 5;
			this.button_maxrender.Text = "Render 1280x720";
			this.button_maxrender.UseVisualStyleBackColor = true;
			// 
			// button_screenshot
			// 
			this.button_screenshot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button_screenshot.Location = new System.Drawing.Point(3, 312);
			this.button_screenshot.Name = "button_screenshot";
			this.button_screenshot.Size = new System.Drawing.Size(143, 23);
			this.button_screenshot.TabIndex = 6;
			this.button_screenshot.Text = "Save Result to File";
			this.button_screenshot.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(3, 29);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(146, 248);
			this.tabControl1.TabIndex = 14;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label2);
			this.tabPage1.Controls.Add(this.maxBounceCount);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Controls.Add(this.cameraSpeedScale);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(138, 222);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Render";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(0, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 13);
			this.label2.TabIndex = 10;
			this.label2.Text = "Max. Bounces";
			// 
			// maxBounceCount
			// 
			this.maxBounceCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.maxBounceCount.BackColor = System.Drawing.SystemColors.Window;
			this.maxBounceCount.LargeChange = 1;
			this.maxBounceCount.Location = new System.Drawing.Point(3, 66);
			this.maxBounceCount.Maximum = 4;
			this.maxBounceCount.Name = "maxBounceCount";
			this.maxBounceCount.Size = new System.Drawing.Size(132, 45);
			this.maxBounceCount.TabIndex = 9;
			this.maxBounceCount.Value = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, -1);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Camera velocity";
			// 
			// cameraSpeedScale
			// 
			this.cameraSpeedScale.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cameraSpeedScale.BackColor = System.Drawing.SystemColors.Window;
			this.cameraSpeedScale.Location = new System.Drawing.Point(3, 15);
			this.cameraSpeedScale.Maximum = 20;
			this.cameraSpeedScale.Minimum = 1;
			this.cameraSpeedScale.Name = "cameraSpeedScale";
			this.cameraSpeedScale.Size = new System.Drawing.Size(132, 45);
			this.cameraSpeedScale.TabIndex = 7;
			this.cameraSpeedScale.Value = 10;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.splitContainer2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(138, 222);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Scene";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.sceneTree);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer2.Size = new System.Drawing.Size(138, 222);
			this.splitContainer2.SplitterDistance = 127;
			this.splitContainer2.TabIndex = 1;
			// 
			// sceneTree
			// 
			this.sceneTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.sceneTree.Location = new System.Drawing.Point(3, 3);
			this.sceneTree.Name = "sceneTree";
			treeNode1.Name = "Node1";
			treeNode1.Text = "Node1";
			treeNode2.Name = "Node2";
			treeNode2.Text = "Node2";
			treeNode3.Name = "Node0";
			treeNode3.Text = "Node0";
			treeNode4.Name = "Node4";
			treeNode4.Text = "Node4";
			treeNode5.Name = "Node5";
			treeNode5.Text = "Node5";
			treeNode6.Name = "Node8";
			treeNode6.Text = "Node8";
			treeNode7.Name = "Node6";
			treeNode7.Text = "Node6";
			treeNode8.Name = "Node3";
			treeNode8.Text = "Node3";
			treeNode9.Name = "Node7";
			treeNode9.Text = "Node7";
			this.sceneTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode8,
            treeNode9});
			this.sceneTree.Size = new System.Drawing.Size(132, 121);
			this.sceneTree.TabIndex = 0;
			this.sceneTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnSceneTreeSelect);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.propertiesPanel);
			this.groupBox1.Location = new System.Drawing.Point(0, -1);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(138, 92);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Properties";
			// 
			// propertiesPanel
			// 
			this.propertiesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.propertiesPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.propertiesPanel.Location = new System.Drawing.Point(3, 19);
			this.propertiesPanel.Name = "propertiesPanel";
			this.propertiesPanel.Size = new System.Drawing.Size(132, 70);
			this.propertiesPanel.TabIndex = 0;
			this.propertiesPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.propertiesPanel_Paint);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.sceneSelection);
			this.splitContainer1.Panel1.Controls.Add(this.progressBar);
			this.splitContainer1.Panel1.Controls.Add(this.button_maxrender);
			this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
			this.splitContainer1.Panel1.Controls.Add(this.button_screenshot);
			this.splitContainer1.Panel1.Controls.Add(this.progressInfo);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
			this.splitContainer1.Panel2.Controls.Add(this.imageViewer);
			this.splitContainer1.Size = new System.Drawing.Size(627, 414);
			this.splitContainer1.SplitterDistance = 148;
			this.splitContainer1.TabIndex = 16;
			// 
			// sceneSelection
			// 
			this.sceneSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.sceneSelection.CausesValidation = false;
			this.sceneSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.sceneSelection.Location = new System.Drawing.Point(6, 6);
			this.sceneSelection.Name = "sceneSelection";
			this.sceneSelection.Size = new System.Drawing.Size(135, 21);
			this.sceneSelection.TabIndex = 15;
			this.sceneSelection.TabStop = false;
			this.sceneSelection.DropDownClosed += new System.EventHandler(this.OnDropdownClosed);
			this.sceneSelection.SelectedValueChanged += new System.EventHandler(this.OnChange);
			this.sceneSelection.Enter += new System.EventHandler(this.OnFocusEnter);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.toolStripStatusLabel1,
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2,
            this.toolStripSplitButton2});
			this.statusStrip1.Location = new System.Drawing.Point(0, 392);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(475, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripSplitButton1
			// 
			this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bbToolStripMenuItem,
            this.aaToolStripMenuItem});
			this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
			this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSplitButton1.Name = "toolStripSplitButton1";
			this.toolStripSplitButton1.Size = new System.Drawing.Size(32, 20);
			this.toolStripSplitButton1.Text = "toolStripSplitButton1";
			// 
			// bbToolStripMenuItem
			// 
			this.bbToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aaToolStripMenuItem1,
            this.vvToolStripMenuItem});
			this.bbToolStripMenuItem.Name = "bbToolStripMenuItem";
			this.bbToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
			this.bbToolStripMenuItem.Text = "bb";
			// 
			// aaToolStripMenuItem1
			// 
			this.aaToolStripMenuItem1.Name = "aaToolStripMenuItem1";
			this.aaToolStripMenuItem1.Size = new System.Drawing.Size(86, 22);
			this.aaToolStripMenuItem1.Text = "aa";
			// 
			// vvToolStripMenuItem
			// 
			this.vvToolStripMenuItem.Name = "vvToolStripMenuItem";
			this.vvToolStripMenuItem.Size = new System.Drawing.Size(86, 22);
			this.vvToolStripMenuItem.Text = "vv";
			// 
			// aaToolStripMenuItem
			// 
			this.aaToolStripMenuItem.Name = "aaToolStripMenuItem";
			this.aaToolStripMenuItem.Size = new System.Drawing.Size(88, 22);
			this.aaToolStripMenuItem.Text = "aa";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 20);
			this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
			// 
			// toolStripDropDownButton2
			// 
			this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
			this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
			this.toolStripDropDownButton2.Size = new System.Drawing.Size(29, 20);
			this.toolStripDropDownButton2.Text = "toolStripDropDownButton2";
			// 
			// toolStripSplitButton2
			// 
			this.toolStripSplitButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripSplitButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton2.Image")));
			this.toolStripSplitButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSplitButton2.Name = "toolStripSplitButton2";
			this.toolStripSplitButton2.Size = new System.Drawing.Size(32, 20);
			this.toolStripSplitButton2.Text = "toolStripSplitButton2";
			// 
			// imageViewer
			// 
			this.imageViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.imageViewer.Location = new System.Drawing.Point(3, 3);
			this.imageViewer.Name = "imageViewer";
			this.imageViewer.Size = new System.Drawing.Size(469, 408);
			this.imageViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.imageViewer.TabIndex = 0;
			this.imageViewer.TabStop = false;
			// 
			// RaytracerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(627, 414);
			this.Controls.Add(this.splitContainer1);
			this.Name = "RaytracerForm";
			this.Text = "RaytracerForm";
			this.Load += new System.EventHandler(this.RaytracerForm_Load);
			this.Enter += new System.EventHandler(this.OnFocusEnter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.maxBounceCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cameraSpeedScale)).EndInit();
			this.tabPage2.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageViewer)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		public SceneViewerPictureBox imageViewer;
		public ProgressBar progressBar;
		public RichTextBox progressInfo;
		public Button button_maxrender;
		public Button button_screenshot;
		public TabControl tabControl1;
		public TabPage tabPage1;
		public Label label2;
		public TrackBar maxBounceCount;
		public Label label1;
		public TrackBar cameraSpeedScale;
		public TabPage tabPage2;
		public TreeView sceneTree;
		private SplitContainer splitContainer1;
		private SplitContainer splitContainer2;
		private GroupBox groupBox1;
		public FlowLayoutPanel propertiesPanel;
		private StatusStrip statusStrip1;
		private ToolStripSplitButton toolStripSplitButton1;
		private ToolStripStatusLabel toolStripStatusLabel1;
		private ToolStripDropDownButton toolStripDropDownButton1;
		private ToolStripDropDownButton toolStripDropDownButton2;
		private ToolStripSplitButton toolStripSplitButton2;
		private ToolStripMenuItem bbToolStripMenuItem;
		private ToolStripMenuItem aaToolStripMenuItem1;
		private ToolStripMenuItem vvToolStripMenuItem;
		private ToolStripMenuItem aaToolStripMenuItem;
		private ComboBox sceneSelection;
	}
}