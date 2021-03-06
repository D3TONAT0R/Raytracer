
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
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.progressInfo = new System.Windows.Forms.RichTextBox();
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
			this.imageViewer = new Raytracer.SceneViewerPictureBox();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.asasToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.openSceneMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openSampleSceneMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveSceneAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startRenderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renderToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveCurrentViewToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cancelCurrentRenderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.resolutionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.qualityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.defaultToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.test1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.test11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
			((System.ComponentModel.ISupportInitialize)(this.imageViewer)).BeginInit();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(3, 364);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(143, 23);
			this.progressBar.TabIndex = 1;
			// 
			// progressInfo
			// 
			this.progressInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.progressInfo.Location = new System.Drawing.Point(3, 305);
			this.progressInfo.Name = "progressInfo";
			this.progressInfo.ReadOnly = true;
			this.progressInfo.Size = new System.Drawing.Size(143, 53);
			this.progressInfo.TabIndex = 4;
			this.progressInfo.Text = "Render Info\n...\n...\n...";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(3, 3);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(146, 296);
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
			this.tabPage1.Size = new System.Drawing.Size(138, 270);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Camera";
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
			this.label1.Location = new System.Drawing.Point(0, 0);
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
			this.tabPage2.Size = new System.Drawing.Size(138, 260);
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
			this.splitContainer2.Size = new System.Drawing.Size(138, 260);
			this.splitContainer2.SplitterDistance = 148;
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
			this.sceneTree.Size = new System.Drawing.Size(132, 142);
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
			this.groupBox1.Size = new System.Drawing.Size(138, 109);
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
			this.propertiesPanel.Size = new System.Drawing.Size(132, 87);
			this.propertiesPanel.TabIndex = 0;
			this.propertiesPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.propertiesPanel_Paint);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.progressBar);
			this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
			this.splitContainer1.Panel1.Controls.Add(this.progressInfo);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.imageViewer);
			this.splitContainer1.Size = new System.Drawing.Size(627, 390);
			this.splitContainer1.SplitterDistance = 148;
			this.splitContainer1.TabIndex = 16;
			// 
			// imageViewer
			// 
			this.imageViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.imageViewer.Cursor = System.Windows.Forms.Cursors.Cross;
			this.imageViewer.Location = new System.Drawing.Point(3, 3);
			this.imageViewer.Name = "imageViewer";
			this.imageViewer.Size = new System.Drawing.Size(469, 384);
			this.imageViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.imageViewer.TabIndex = 0;
			this.imageViewer.TabStop = false;
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asasToolStripMenuItem2,
            this.renderToolStripMenuItem,
            this.testToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(627, 24);
			this.menuStrip.TabIndex = 17;
			this.menuStrip.Text = "menuStrip";
			// 
			// asasToolStripMenuItem2
			// 
			this.asasToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSceneMenuItem,
            this.openSampleSceneMenuItem,
            this.saveSceneAsToolStripMenuItem,
            this.quitToolStripMenuItem});
			this.asasToolStripMenuItem2.Name = "asasToolStripMenuItem2";
			this.asasToolStripMenuItem2.Size = new System.Drawing.Size(37, 20);
			this.asasToolStripMenuItem2.Text = "File";
			// 
			// openSceneMenuItem
			// 
			this.openSceneMenuItem.Name = "openSceneMenuItem";
			this.openSceneMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openSceneMenuItem.Size = new System.Drawing.Size(254, 22);
			this.openSceneMenuItem.Text = "Open Scene ...";
			this.openSceneMenuItem.Click += new System.EventHandler(this.OnOpenSceneMenuItemClick);
			// 
			// openSampleSceneMenuItem
			// 
			this.openSampleSceneMenuItem.Name = "openSampleSceneMenuItem";
			this.openSampleSceneMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
			this.openSampleSceneMenuItem.Size = new System.Drawing.Size(254, 22);
			this.openSampleSceneMenuItem.Text = "Open Sample Scene";
			this.openSampleSceneMenuItem.Click += new System.EventHandler(this.OnOpenSampleSceneMenuItemClick);
			// 
			// saveSceneAsToolStripMenuItem
			// 
			this.saveSceneAsToolStripMenuItem.Name = "saveSceneAsToolStripMenuItem";
			this.saveSceneAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.saveSceneAsToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.saveSceneAsToolStripMenuItem.Text = "Save Scene As ...";
			this.saveSceneAsToolStripMenuItem.Click += new System.EventHandler(this.OnSaveSceneAsMenuItemClick);
			// 
			// quitToolStripMenuItem
			// 
			this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
			this.quitToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
			this.quitToolStripMenuItem.Text = "Quit";
			this.quitToolStripMenuItem.Click += new System.EventHandler(this.OnQuitMenuItemClick);
			// 
			// renderToolStripMenuItem
			// 
			this.renderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startRenderToolStripMenuItem,
            this.renderToFileToolStripMenuItem,
            this.saveCurrentViewToFileToolStripMenuItem,
            this.cancelCurrentRenderToolStripMenuItem,
            this.toolStripMenuItem1,
            this.resolutionMenuItem,
            this.qualityMenuItem});
			this.renderToolStripMenuItem.Name = "renderToolStripMenuItem";
			this.renderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.renderToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.renderToolStripMenuItem.Text = "Render";
			// 
			// startRenderToolStripMenuItem
			// 
			this.startRenderToolStripMenuItem.Name = "startRenderToolStripMenuItem";
			this.startRenderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.startRenderToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.startRenderToolStripMenuItem.Text = "Render to Screen";
			this.startRenderToolStripMenuItem.Click += new System.EventHandler(this.OnRenderMenuItemClick);
			// 
			// renderToFileToolStripMenuItem
			// 
			this.renderToFileToolStripMenuItem.Name = "renderToFileToolStripMenuItem";
			this.renderToFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.R)));
			this.renderToFileToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.renderToFileToolStripMenuItem.Text = "Render to File";
			this.renderToFileToolStripMenuItem.Click += new System.EventHandler(this.OnRenderToFileMenuItemClick);
			// 
			// saveCurrentViewToFileToolStripMenuItem
			// 
			this.saveCurrentViewToFileToolStripMenuItem.Name = "saveCurrentViewToFileToolStripMenuItem";
			this.saveCurrentViewToFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.saveCurrentViewToFileToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.saveCurrentViewToFileToolStripMenuItem.Text = "Save Current View To File";
			this.saveCurrentViewToFileToolStripMenuItem.Click += new System.EventHandler(this.OnSaveViewMenuItemClick);
			// 
			// cancelCurrentRenderToolStripMenuItem
			// 
			this.cancelCurrentRenderToolStripMenuItem.Name = "cancelCurrentRenderToolStripMenuItem";
			this.cancelCurrentRenderToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.cancelCurrentRenderToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
			this.cancelCurrentRenderToolStripMenuItem.Text = "Cancel Current Render";
			this.cancelCurrentRenderToolStripMenuItem.Click += new System.EventHandler(this.OnCancelRenderMenuItemClick);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(243, 6);
			// 
			// resolutionMenuItem
			// 
			this.resolutionMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultToolStripMenuItem});
			this.resolutionMenuItem.Name = "resolutionMenuItem";
			this.resolutionMenuItem.Size = new System.Drawing.Size(246, 22);
			this.resolutionMenuItem.Text = "Resolution";
			// 
			// defaultToolStripMenuItem
			// 
			this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
			this.defaultToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.defaultToolStripMenuItem.Text = "Default";
			// 
			// qualityMenuItem
			// 
			this.qualityMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultToolStripMenuItem1});
			this.qualityMenuItem.Name = "qualityMenuItem";
			this.qualityMenuItem.Size = new System.Drawing.Size(246, 22);
			this.qualityMenuItem.Text = "Quality Preset";
			// 
			// defaultToolStripMenuItem1
			// 
			this.defaultToolStripMenuItem1.Name = "defaultToolStripMenuItem1";
			this.defaultToolStripMenuItem1.Size = new System.Drawing.Size(112, 22);
			this.defaultToolStripMenuItem1.Text = "Default";
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.test1ToolStripMenuItem});
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
			this.testToolStripMenuItem.Text = "test";
			// 
			// test1ToolStripMenuItem
			// 
			this.test1ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.test11ToolStripMenuItem});
			this.test1ToolStripMenuItem.Name = "test1ToolStripMenuItem";
			this.test1ToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
			this.test1ToolStripMenuItem.Text = "test1";
			// 
			// test11ToolStripMenuItem
			// 
			this.test11ToolStripMenuItem.Name = "test11ToolStripMenuItem";
			this.test11ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			this.test11ToolStripMenuItem.Text = "test11";
			// 
			// RaytracerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(627, 414);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip);
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
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.imageViewer)).EndInit();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public SceneViewerPictureBox imageViewer;
		public ProgressBar progressBar;
		public RichTextBox progressInfo;
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
		private ToolStripMenuItem asasToolStripMenuItem2;
		private ToolStripMenuItem openSceneMenuItem;
		private ToolStripMenuItem openSampleSceneMenuItem;
		private ToolStripMenuItem renderToolStripMenuItem;
		private ToolStripMenuItem startRenderToolStripMenuItem;
		private ToolStripMenuItem renderToFileToolStripMenuItem;
		private ToolStripSeparator toolStripMenuItem1;
		private ToolStripMenuItem testToolStripMenuItem;
		private ToolStripMenuItem test1ToolStripMenuItem;
		private ToolStripMenuItem test11ToolStripMenuItem;
		private ToolStripMenuItem defaultToolStripMenuItem;
		private ToolStripMenuItem defaultToolStripMenuItem1;
		internal ToolStripMenuItem resolutionMenuItem;
		internal ToolStripMenuItem qualityMenuItem;
		internal MenuStrip menuStrip;
		private ToolStripMenuItem saveSceneAsToolStripMenuItem;
		private ToolStripMenuItem cancelCurrentRenderToolStripMenuItem;
		private ToolStripMenuItem quitToolStripMenuItem;
		private ToolStripMenuItem saveCurrentViewToFileToolStripMenuItem;
	}
}