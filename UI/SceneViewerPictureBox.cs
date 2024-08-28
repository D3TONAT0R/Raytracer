using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace Raytracer {
	/// <summary>
	/// Inherits from PictureBox; adds Interpolation Mode Setting
	/// </summary>
	public class SceneViewerPictureBox : PictureBox {

		public Point? hoveredPixelCoordinates;
		public Label coordinateLabel;

		public SceneViewerPictureBox() : base()
		{
			Cursor = Cursors.Cross;
			coordinateLabel = new Label();
			Controls.Add(coordinateLabel);
			coordinateLabel.BackColor = System.Drawing.Color.Transparent;
			coordinateLabel.ForeColor = System.Drawing.Color.FromArgb(128, 128, 128);
			coordinateLabel.Font = new Font(Font, FontStyle.Bold);
			coordinateLabel.BringToFront();
			coordinateLabel.Size = new Size(200, 50);
			coordinateLabel.Enabled = true;
		}

		protected override void OnPaint(PaintEventArgs paintEventArgs) {
			try {
				paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				base.OnPaint(paintEventArgs);
			} catch {

			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(Image == null) return;
			Point? pixel = GetPixelFromScreenLocation(e.Location);
			UpdateLabel(pixel);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			if(RaytracerEngine.Scene == null) return;
			var pixel = GetPixelFromScreenLocation(e.Location);
			if(!pixel.HasValue) return;
			var viewportCoord = new Vector2(pixel.Value.X / (float)Image.Size.Width, pixel.Value.Y / (float)Image.Size.Height) * 2f - Vector2.One;
			viewportCoord.Y = -viewportCoord.Y;
			if(e.Button == MouseButtons.Left)
			{
				PickObject(viewportCoord);
			}
			else if(e.Button == MouseButtons.Right)
			{
#if DEBUG
				System.Diagnostics.Debugger.Break();
#endif
				PickObject(viewportCoord);
			}
		}

		private void PickObject(Vector2 viewportCoord)
		{
			var ray = Camera.MainCamera.ScreenPointToRay(viewportCoord);
			bool hit = SceneRenderer.TraceRay(RaytracerEngine.Scene, ref ray, VisibilityFlags.Direct, out var result, optimize: false);
			TreeNode nextSelection = null;
			if(hit && result.HitShape != null)
			{
				nextSelection = RaytracerEngine.infoWindow.FindNode(result.HitShape);
				if (nextSelection == null)
				{
					//Try to pick parent object
					nextSelection = RaytracerEngine.infoWindow.FindNode(result.HitShape.parent);
				}
			}
			RaytracerEngine.infoWindow.sceneTree.SelectedNode = nextSelection;
		}

		private void UpdateLabel(Point? hoveredPixel)
		{
			if(hoveredPixel != null)
			{
				coordinateLabel.Text = $"Pixel: [{hoveredPixel.Value.X},{hoveredPixel.Value.Y}]";
			}
			else
			{
				coordinateLabel.Text = "";
			}
		}

		private Point? GetPixelFromScreenLocation(Point screenLocation)
		{
			var size = GetDisplayedImageSize();
			screenLocation.Y -= (ClientSize.Height - size.Height) / 2;
			screenLocation.X -= (ClientSize.Width - size.Width) / 2;
			Vector2 pos = new Vector2(screenLocation.X / (float)size.Width, screenLocation.Y / (float)size.Height);
			Point pixel = new Point((int)(pos.X * Image.Width), (int)(pos.Y * Image.Height));
			if(pixel.X >= 0 && pixel.Y >= 0 && pixel.X < Image.Width && pixel.Y < Image.Height)
			{
				return pixel;
			}
			else
			{
				return null;
			}
		}

		protected override void OnMouseLeave(EventArgs e) {
			hoveredPixelCoordinates = null;
		}

		private Size GetDisplayedImageSize() {
			Size containerSize = ClientSize;
			float containerAspectRatio = (float)containerSize.Height / (float)containerSize.Width;
			Size originalImageSize = Image.Size;
			float imageAspectRatio = (float)originalImageSize.Height / (float)originalImageSize.Width;

			Size result = new Size();
			if(containerAspectRatio > imageAspectRatio) {
				result.Width = containerSize.Width;
				result.Height = (int)(imageAspectRatio * (float)containerSize.Width);
			} else {
				result.Height = containerSize.Height;
				result.Width = (int)((1.0f / imageAspectRatio) * (float)containerSize.Height);
			}
			return result;
		}

		private void InitializeComponent()
		{
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			// 
			// SceneViewerPictureBox
			// 
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);

		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{

		}
	}
}
