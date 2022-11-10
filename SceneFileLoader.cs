using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Raytracer
{

	public static class SceneFileLoader
	{
		enum DataBlockType
		{
			Environment,
			Colors,
			Materials,
			Objects
		}

		internal abstract class Content
		{
			public string keyword;
		}

		internal class StringContent : Content
		{
			public string data;

			public StringContent(string line)
			{
				line = TrimLine(line);
				var split = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
				keyword = split[0];
				data = line.Substring(split[0].Length+1);
			}
		}

		internal class BlockContent : Content
		{
			public string name;
			public string refName;
			public List<Content> data = new List<Content>();

			public BlockContent(string currentLine, StringReader reader, ref int lineNum)
			{
				int startLineNum = lineNum;
				var split = TrimLine(currentLine).Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
				if (!split[split.Length - 1].StartsWith("{")) throw new FormatException($"Missing '{{' at line {lineNum}");
				keyword = split[0];
				if(keyword.Contains('('))
				{
					var match = Regex.Match(keyword, @"\(([^)]+)\)");
					refName = match.Value.Substring(1, match.Value.Length - 2);
					keyword = keyword.Split('(')[0];
				}
				if(split.Length > 2)
				{
					name = split[1];
				}
				while (true)
				{
					var line = reader.ReadLine();
					if(line.StartsWith("#")) continue;
					if (line == null) throw new EndOfStreamException($"Unclosed block at line {startLineNum}.");
					if(line.Contains('}'))
					{
						//End of block reached
						break;
					}
					if(line.Contains('{'))
					{
						data.Add(new BlockContent(line, reader, ref lineNum));
					}
					else if(!string.IsNullOrWhiteSpace(line))
					{
						data.Add(new StringContent(line));
					}
				}
			}

			public string[] StringsToArray()
			{
				var arr = new string[data.Count];
				for (int i = 0; i < data.Count; i++)
				{
					var sc = data[i] as StringContent;
					arr[i] = $"{sc.keyword} {sc.data}" ?? "";
				}
				return arr;
			}
		}

		private static string TrimLine(string line)
		{
			line = line.Trim();
			line = line.Replace('\t', ' ');
			while (line.Contains("  ")) line = line.Replace("  ", " ");
			return line;
		}

		public static Scene CreateFromFile(string filePath)
		{
			string sceneName = null;
			var reader = new StringReader(File.ReadAllText(filePath));
			int lineNum = 0;
			List<BlockContent> blocks = new List<BlockContent>();
			while(true)
			{
				var line = reader.ReadLine();
				lineNum++;
				if(line != null)
				{
					//Skip empty lines and comments
					if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

					if(line.StartsWith("NAME"))
					{
						if (sceneName != null) throw new ArgumentException($"Duplicate NAME tag at line {lineNum}.");
						sceneName = line.Substring(5);
						continue;
					}

					blocks.Add(new BlockContent(line, reader, ref lineNum));
				}
				else
				{
					break;
				}
			}
			if (string.IsNullOrWhiteSpace(sceneName)) sceneName = "Untitled";

			var scene = new Scene(sceneName);

			var envBlock = blocks.Find((b) => b.keyword == "ENVIRONMENT");
			var colBlock = blocks.Find((b) => b.keyword == "COLORS");
			var matBlock = blocks.Find((b) => b.keyword == "MATERIALS");
			var prefabBlock = blocks.Find((b) => b.keyword == "PREFABS");
			var objBlock = blocks.Find((b) => b.keyword == "OBJECTS");

			if (envBlock != null) scene.environment = ParseEnvironmentBlock(envBlock);
			if (colBlock != null) scene.globalColors = ParseColorsBlock(colBlock);
			if (matBlock != null) scene.globalMaterials = ParseMaterialsBlock(matBlock, scene);
			if (prefabBlock != null) scene.prefabContent = ParseObjectsBlock(prefabBlock, scene);
			if (objBlock != null) scene.sceneContent = ParseObjectsBlock(objBlock, scene);

			return scene;
		}

		static Environment ParseEnvironmentBlock(BlockContent block)
		{
			var env = new Environment();
			foreach(var d in block.data)
			{
				var line = d as StringContent;
				switch(line.keyword)
				{
					case "SKYBOX":
						env.skyboxTexture = Sampler2D.Create(line.data);
						break;
					case "SKYBOX_SPHERICAL":
						env.skyboxIsSpherical = bool.Parse(line.data);
						break;
					case "AMBIENT":
						env.ambientColor = Color.Parse(line.data);
						break;
					case "FOG":
						env.fogColor = Color.Parse(line.data);
						break;
					case "FOG_DISTANCE":
						env.fogDistance = float.Parse(line.data);
						break;
					default:
						throw new ArgumentException($"Unexpected keyword '{line.keyword}' in ENVIRONMENT block.");
				}
			}
			return env;
		}

		static Dictionary<string, Color> ParseColorsBlock(BlockContent block)
		{
			var colors = new Dictionary<string, Color>();
			foreach(var d in block.data)
			{
				var l = d as StringContent;
				colors.Add(l.keyword, Color.Parse(l.data));
			}
			return colors;
		}

		static Dictionary<string, Material> ParseMaterialsBlock(BlockContent block, Scene scene)
		{
			var materials = new Dictionary<string, Material>();
			foreach (var d in block.data)
			{
				var l = d as BlockContent;
				var mat = Reflection.CreateMaterial(l, scene);
				mat.globalMaterialName = l.keyword;
				mat.isGlobalMaterial = true;
				materials.Add(l.keyword, mat);
			}
			return materials;
		}

		static List<SceneObject> ParseObjectsBlock(BlockContent block, Scene scene)
		{
			var list = new List<SceneObject>();
			foreach(var c in block.data)
			{
				var b = c as BlockContent;

				var so = Reflection.CreateSceneObject(scene, b);
				list.Add(so);
			}
			return list;
		}
	}
}
