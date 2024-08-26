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

		public SceneViewerPictureBox() : base() {
			Cursor = Cursors.Cross;
			coordinateLabel = new Label();
			Controls.Add(coordinateLabel);
			coordinateLabel.BackColor = System.Drawing.Color.Transparent;
			coordinateLabel.ForeColor = System.Drawing.Color.Blue;
			coordinateLabel.Font = new Font(Font, FontStyle.Bold);
			coordinateLabel.BringToFront();
			coordinateLabel.Size = new Size(200, 50);
			coordinateLabel.Enabled = false;
		}

		protected override void OnPaint(PaintEventArgs paintEventArgs) {
			try {
				paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				base.OnPaint(paintEventArgs);
			} catch {

			}
		}

		private void UpdateLabel() {
			if(hoveredPixelCoordinates != null) {
				Point pixel = (Point)hoveredPixelCoordinates;
				coordinateLabel.Text = $"Pixel: [{pixel.X},{pixel.Y}]";
			} else {
				coordinateLabel.Text = "";
			}
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			if(Image == null) return;
			Point loc = e.Location;
			
			UpdateLabel();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			var pixel = GetPixelFromScreenLocation(e.Location);
			if(!pixel.HasValue) return;
			var viewportCoord = new Vector2(pixel.Value.X / (float)Image.Size.Width, pixel.Value.Y / (float)Image.Size.Height) * 2f - Vector2.One;
			viewportCoord.Y = -viewportCoord.Y;
			var ray = Camera.MainCamera.ScreenPointToRay(viewportCoord);
			if(e.Button == MouseButtons.Left)
			{
				var pos = SceneRenderer.TraceRay(RaytracerEngine.Scene, ref ray, VisibilityFlags.Direct, out var hit, allowOptimization: false);
				TreeNode nextSelection = null;
				if(hit != null)
				{
					nextSelection = RaytracerEngine.infoWindow.FindNode(hit);
				}
				RaytracerEngine.infoWindow.sceneTree.SelectedNode = nextSelection;
			}
			else if(e.Button == MouseButtons.Right)
			{
#if DEBUG
				System.Diagnostics.Debugger.Break();
#endif
				var color = SceneRenderer.TraceRay(RaytracerEngine.Scene, ray, VisibilityFlags.Direct);
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
	}
}
