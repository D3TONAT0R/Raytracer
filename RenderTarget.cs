using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	public class RenderTarget
	{
		public string name;

		public readonly int width;
		public readonly int height;
		public int PixelCount => width * height;

		public RenderTarget(string name, int w, int h)
		{
			this.name = name;
			width = w;
			height = h;
		}

		private Bitmap _renderBuffer;
		public Bitmap RenderBuffer
		{
			get
			{
				if (_renderBuffer == null)
				{
					_renderBuffer = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				}
				return _renderBuffer;
			}
		}
	}
}
