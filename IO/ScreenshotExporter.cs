using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Raytracer
{
	public static class ScreenshotExporter
	{
		public static string ScreenshotRootFolder => Path.Combine(PersistentPrefs.ResourcesRootPath, "Screenshots");
		public static string AnimationsRootFolder => Path.Combine(PersistentPrefs.ResourcesRootPath, "Animations");

		public static string currentAnimationSequenceName = null;

		public static void SaveScreenshot(bool useSceneFileName = true, string name = null)
		{
			string fullName = "";
			if(useSceneFileName)
			{
				fullName = RaytracerEngine.Scene.SourceFileName;
			}
			if(!string.IsNullOrEmpty(name))
			{
				if(fullName.Length > 0) fullName += "-";
				fullName += name;
			}
			SaveScreenshot(GetActiveBuffer(), ScreenshotRootFolder, fullName);
		}

		public static void SaveSequenceFrame()
		{
			if(currentAnimationSequenceName == null)
			{
				throw new NullReferenceException("No sequence has been started.");
			}
			SaveScreenshot(GetActiveBuffer(), Path.Combine(AnimationsRootFolder, currentAnimationSequenceName), "anim");
		}

		public static void BeginNewSequence(string name)
		{
			int animationID = 1;
			while(Directory.Exists(Path.Combine(AnimationsRootFolder, $"{name}-{animationID:D4}")))
			{
				animationID++;
			}
			currentAnimationSequenceName = $"{name}_{animationID:D4}";
		}

		public static void SaveScreenshot(Bitmap buffer, string path, string name)
		{
			if(buffer != null)
			{
				int num = 1;
				string filename = GetPathSafeString(name);
				var fullpath = Path.Combine(path, filename + "-");
				while(File.Exists(fullpath + num.ToString("D4") + ".png"))
				{
					num++;
				}
				fullpath = fullpath + num.ToString("D4") + ".png";
				string dir = Path.GetDirectoryName(fullpath);
				if(!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				buffer.Save(fullpath);
			}
		}

		private static Bitmap GetActiveBuffer()
		{
			var target = RaytracerEngine.lastRenderTarget ?? RaytracerEngine.CurrentRenderTarget;
			return target.RenderBuffer;
		}

		private static string GetPathSafeString(string filename)
		{
			foreach(var c in Path.GetInvalidFileNameChars())
			{
				filename = filename.Replace(c, '-');
			}
			return filename;
		}
	}
}
