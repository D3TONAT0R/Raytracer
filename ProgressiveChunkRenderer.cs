using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Raytracer
{
	public class ProgressiveChunkRenderer : ScreenRenderer
	{
		private struct Chunk
		{
			public int xMin;
			public int xMax;
			public int yMin;
			public int yMax;
			public int cellSize;

			public int Area => (yMax - yMin + 1) * (xMax - xMin + 1);

			public Chunk(int x, int y, int size, int imgWidth, int imgHeight)
			{
				cellSize = size;
				xMin = x;
				xMax = Math.Min(x + size - 1, imgWidth - 1);
				yMin = y;
				yMax = Math.Min(y + size - 1, imgHeight - 1);
			}

			public void SplitNextIteration(List<Chunk> newList, int imgWidth, int imgHeight)
			{
				if (cellSize <= ProgressiveChunkRenderer.cellSize) throw new InvalidOperationException("New cell size is less or equal to constant 'cellSize'.");
				var newCellSize = (int)Math.Sqrt(cellSize);
				Split(newList, newCellSize, imgWidth, imgHeight);
			}

			public void Split(List<Chunk> newList, int newCellSize, int imgWidth, int imgHeight)
			{
				int width = xMax - xMin + 1;
				int height = yMax - yMin + 1;
				int cx = (int)Math.Ceiling(width / (double)newCellSize);
				int cy = (int)Math.Ceiling(height / (double)newCellSize);
				for (int y = 0; y < cy; y++)
				{
					for (int x = 0; x < cx; x++)
					{
						newList.Add(new Chunk(xMin + x * newCellSize, yMin + y * newCellSize, newCellSize, imgWidth, imgHeight));
					}
				}
			}
		}

		const int passCount = 3;
		const int cellSize = 4;
		const int minChunkSize = 16;

		private List<Chunk> currentIterationChunks = new List<Chunk>();
		private List<Chunk> nextIterationChunks = new List<Chunk>();
		private int pass;

		private int targetWidth;
		private int targetHeight;
		private int targetDepth;
		private Camera currentCamera;
		private Scene currentScene;
		private byte[] currentBuffer;

		private int chunksRendered = 0;
		private int pixelsRendered = 0;

		private static object chunkLock = new object();

		public override void GetProgressInfo(out string progressString, out float progress)
		{
			progressString = $"Pass {(passCount - pass)}/{passCount}: {chunksRendered}/{currentIterationChunks.Count}";
			progress = pixelsRendered / (float)CurrentPixelRenderData.ScreenArea;
		}

		public override void RenderToScreen(Camera camera, Scene scene, byte[] byteBuffer, int width, int height, int pixelDepth)
		{
			pixelsRendered = 0;

			currentCamera = camera;
			currentScene = scene;
			currentBuffer = byteBuffer;
			targetWidth = width;
			targetHeight = height;
			targetDepth = pixelDepth;

			pass = passCount;

			InitializeFullscreenChunkArea();
			bool firstPass = true;
			while (pass >= 0)
			{
				chunksRendered = 0;
				nextIterationChunks.Clear();
				int i = 0;
				Parallel.For(i, currentIterationChunks.Count, ci => RenderChunk(currentIterationChunks[ci], pass, firstPass));
				firstPass = false;
				currentIterationChunks.Clear();
				currentIterationChunks.AddRange(nextIterationChunks);
				pass--;
			}
		}

		private void RenderChunk(Chunk c, int pass, bool isFirstPass)
		{
			//Render
			int s = Pow(cellSize, pass);
			int rendered = 0;
			for (int y = c.yMin; y <= c.yMax; y += s)
			{
				for (int x = c.xMin; x <= c.xMax; x += s)
				{
					if (isFirstPass && x == c.xMin && y == c.yMin) continue;
					var col = GetPixelColor(x, y, currentCamera, currentScene);
					if (s <= 1)
					{
						SetPixel(currentBuffer, x, y, col, targetWidth, targetHeight, targetDepth);
					}
					else
					{
						FillPixels(currentBuffer, x, y, x + s - 1, y + s - 1, col, targetWidth, targetHeight, targetDepth);
					}
					rendered++;
				}
			}
			chunksRendered++;
			pixelsRendered += rendered;

			lock(chunkLock)
			{
				if(pass > 1 && Pow(cellSize, pass) > minChunkSize)
				{
					c.SplitNextIteration(nextIterationChunks, targetWidth, targetHeight);
				}
				else
				{
					nextIterationChunks.Add(c);
				}
			}
			/*
			//Split
			if (s > cellSize)
			{
				c.SplitNextIteration(nextIterationChunks, targetWidth, targetHeight);
			}
			else
			{
				//Draw it again, with per-pixel resolution next time
				nextIterationChunks.Add(c);
			}
			*/
		}

		private void InitializeFullscreenChunkArea()
		{
			currentIterationChunks.Clear();
			int size = Pow(cellSize, pass + 1);
			for (int y = 0; y < targetHeight; y += size)
			{
				for (int x = 0; x < targetWidth; x += size)
				{
					var chunk = new Chunk(x, y, size, targetWidth, targetHeight);
					currentIterationChunks.Add(chunk);
				}
			}
		}

		private int Pow(int v, int e)
		{
			if (e == 0) return 1;
			int r = v;
			for (int i = 1; i < e; i++)
			{
				r *= v;
			}
			return r;
		}
	}
}
