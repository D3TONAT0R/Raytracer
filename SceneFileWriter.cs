using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
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

		static Dictionary<Type, SceneObject> defaultInstances = new Dictionary<Type, SceneObject>();

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
			if (mat.indexOfRefraction >= 0) Write("IOR", mat.indexOfRefraction);
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
				WriteObject(obj);
				Blank();
			}
			EndBlock();
		}

		static void WriteObject(SceneObject obj)
		{
			var objType = obj.GetType();
			var keyword = objType.GetCustomAttribute<ObjectIdentifierAttribute>().identifier;
			BeginBlock(keyword, obj.name);
			var set = Reflection.GetExposedFieldSet(obj.GetType());
			foreach(var f in set.fields)
			{
				var value = objType.GetField(f.Value.fieldName).GetValue(obj);
				if (value != null) {
					if (value.Equals(GetInitialValue(objType, f.Value.fieldName)))
					{
						//Skip values that match an object's initial value
						continue;
					}
					string key = f.Key;
					if(value is Vector3 vec)
					{
						Write(key, $"{vec.X} {vec.Y} {vec.Z}");
					}
					else if(value is Material mat)
					{
						if(mat.isGlobalMaterial)
						{
							Write(key, mat.globalMaterialName);
						}
						else
						{
							WriteMaterial(key, mat);
						}
					}
					else if(value is SolidShape[] arr)
					{
						BeginBlock(key);
						foreach(var solid in arr)
						{
							WriteObject(solid);
						}
						EndBlock();
					}
					else if(value is SceneObject[] children)
					{
						BeginBlock(key);
						foreach(var child in children)
						{
							WriteObject(child);
						}
						EndBlock();
					}
					else
					{
						Write(key, value);
					}
				}
			}
			EndBlock();
		}

		static object GetInitialValue(Type objType, string fieldName)
		{
			SceneObject obj;
			if(!defaultInstances.TryGetValue(objType, out obj))
			{
				obj = Activator.CreateInstance(objType) as SceneObject;
				defaultInstances.Add(objType, obj);
			}
			return objType.GetField(fieldName).GetValue(obj);
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
