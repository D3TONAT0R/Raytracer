using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer
{
	public static class SceneFileWriter
	{
		static StringBuilder file;
		static int indentLevel = 0;
		static Scene scene;

		internal static void SaveSceneAsPrompt()
		{
			var d = new SaveFileDialog()
			{
				DefaultExt = ".txt",
				InitialDirectory = RaytracerEngine.rootPath
			};
			var result = d.ShowDialog();
			if (result == DialogResult.OK)
			{
				try
				{
					SaveSceneAs(RaytracerEngine.Scene, d.FileName);
					MessageBox.Show("Scene saved " + d.FileName);
				}
				catch (Exception e)
				{
					MessageBox.Show($"Failed to save scene file '{Path.GetFileName(d.FileName)}':\n{e.Message}", "Open Scene", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		public static void SaveSceneAs(Scene sceneToSave, string savePath)
		{
			scene = sceneToSave;
			file = new StringBuilder();
			indentLevel = 0;
			Write($"#Scene file saved on {System.DateTime.Now:u}");
			Blank();
			if (!string.IsNullOrWhiteSpace(scene.sceneName))
			{
				Write($"NAME", scene.sceneName);
				Blank();
			}
			WriteEnvironmentBlock();
			Blank();
			if (scene.globalColors.Count > 0)
			{
				WriteColorsBlock();
				Blank();
			}
			if (scene.globalMaterials.Count > 0)
			{
				WriteMaterialsBlock();
				Blank();
			}
			WriteObjectsBlock();
			File.WriteAllText(savePath, file.ToString());
			scene = null;
			file = null;
			indentLevel = 0;
		}

		private static void WriteColorsBlock()
		{
			BeginBlock("COLORS");
			foreach (var c in scene.globalColors)
			{
				Write(c.Key, c.Value);
			}
			EndBlock();
		}

		private static void WriteMaterialsBlock()
		{
			BeginBlock("MATERIALS");
			foreach (var c in scene.globalMaterials)
			{
				WriteMaterial(c.Key, c.Value);
			}
			EndBlock();
		}

		private static void WriteMaterial(string name, Material mat)
		{
			BeginBlock(name);
			Write("COLOR", mat.mainColor);
			if (mat.mainTexture != null) Write("MAINTEX", mat.mainTexture);
			if (mat.textureTiling != TilingVector.defaultVector) Write("TILING", mat.textureTiling);
			if (mat.mappingType != TextureMappingType.WorldXYZ) Write("MAPPING", mat.mappingType);
			if (mat.reflectivity != 0) Write("REFL", mat.reflectivity);
			if (mat.smoothness != 0) Write("SMOOTH", mat.smoothness);
			if (mat.refraction != 0) Write("REFRACTION", mat.refraction);
			EndBlock();
		}

		private static void WriteEnvironmentBlock()
		{
			BeginBlock("ENVIRONMENT");
			if (scene.environment.skyboxTexture != null)
			{
				Write("SKYBOX", scene.environment.skyboxTexture);
				Write("SKYBOX_SPHERICAL", scene.environment.skyboxIsSpherical);
			}
			Write("AMBIENT", scene.environment.ambientColor);
			Write("FOG", scene.environment.fogColor);
			Write("FOG_DISTANCE", scene.environment.fogDistance);
			EndBlock();
		}

		private static void WriteObjectsBlock()
		{
			BeginBlock("OBJECTS");
			foreach(var obj in scene.sceneContent)
			{
				//TODO write objects
			}
			EndBlock();
		}

		static void BeginBlock(string keyword, string name = null)
		{
			AppendTabulation();
			file.Append(keyword + " ");
			if(!string.IsNullOrWhiteSpace(name))
			{
				file.Append(name + " ");
			}
			file.AppendLine("{");
			indentLevel++;
		}

		static void EndBlock()
		{
			indentLevel--;
			AppendTabulation();
			file.AppendLine("}");
		}

		static void Blank()
		{
			AppendTabulation();
			file.AppendLine();
		}

		static void Write(string s)
		{
			AppendTabulation();
			file.AppendLine(s);
		}

		static void Write(string name, object value)
		{
			AppendTabulation();
			file.Append(name);
			file.Append(" ");
			file.AppendLine(value.ToString());
		}

		static void AppendTabulation()
		{
			for (int i = 0; i < indentLevel; i++) file.Append("\t");
		}
	}
}
