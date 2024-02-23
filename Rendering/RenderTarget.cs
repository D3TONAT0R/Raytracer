using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	public class RenderTarget
	{
		public string name;

		public readonly int width;
		public readonly int height;

		public readonly int pixelDepth;

		public int PixelCount => width * height;

		private readonly byte[] bitmapData;
		private readonly GCHandle bufferHandle;

		private Bitmap _renderBuffer;
		public Bitmap Bitmap
		{
			get
			{
				if (_renderBuffer == null)
				{
					
				}
				return _renderBuffer;
			}
		}

		public RenderTarget(string name, int w, int h)
		{
			this.name = name;
			width = w;
			height = h;
			pixelDepth = 3;
			bitmapData = new byte[width * height * pixelDepth];
			bufferHandle = GCHandle.Alloc(bitmapData, GCHandleType.Pinned);
			_renderBuffer = new Bitmap(width, height, width * pixelDepth, PixelFormat.Format24bppRgb, bufferHandle.AddrOfPinnedObject());
		}

		public void WritePixel(int x, int y, Color color)
		{
			var bytes = color.ToByteColor();
			if(x < 0 || x >= width || y < 0 || y >= height) throw new IndexOutOfRangeException();
			WritePixelBytes(x, y, bytes);
		}

		private void WritePixelBytes(int x, int y, ByteColor color)
		{
			int y1 = height - y - 1;
			int index = (y1 * width + x) * pixelDepth;
			bitmapData[index + 0] = color.b;
			bitmapData[index + 1] = color.g;
			bitmapData[index + 2] = color.r;
			//Marshal.Copy(pixelByteBuffer, index, buf, pixelDepth);
		}

		public void WritePixelArea(int x1, int y1, int x2, int y2, Color color)
		{
			x1 = Math.Max(x1, 0);
			y1 = Math.Max(y1, 0);
			x2 = Math.Min(x2, width - 1);
			y2 = Math.Min(y2, height - 1);
			var bytes = color.ToByteColor();
			for(int y = y1; y <= y2; y++)
			{
				for(int x = x1; x <= x2; x++)
				{
					WritePixelBytes(x, y, bytes);
				}
			}
		}

		public override string ToString()
		{
			return $"{name} ({width}x{height})";
		}
	}
}
