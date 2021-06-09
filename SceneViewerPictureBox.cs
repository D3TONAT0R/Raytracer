using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;

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
			var size = GetDisplayedImageSize();
			loc.Y -= (ClientSize.Height - size.Height) / 2;
			loc.X -= (ClientSize.Width - size.Width) / 2;
			Vector2 pos = new Vector2(loc.X / (float)size.Width, loc.Y / (float)size.Height);
			Point pixel = new Point((int)(pos.X * Image.Width), (int)(pos.Y * Image.Height));
			if(pixel.X >= 0 && pixel.Y >= 0 && pixel.X < Image.Width && pixel.Y < Image.Height) {
				hoveredPixelCoordinates = pixel;
			} else {
				hoveredPixelCoordinates = null;
			}
			UpdateLabel();
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
