using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{

	public class SceneFileLoader
	{

		string[] fileContents;
		int lineIndex = 0;

		public Dictionary<string, Material> materials = new Dictionary<string, Material>();
		public Dictionary<string, SceneObject> objectList = new Dictionary<string, SceneObject>();

		public Scene CreateFromFile(string sceneName, string[] lines, int index)
		{
			fileContents = lines;
			for (int i = 0; i < lines.Length; i++)
			{
				lines[i].Replace("    ", "\t");
			}
			Scene sn = new Scene(sceneName);
			while (GetLine(lineIndex) != null)
			{
				if (string.IsNullOrWhiteSpace(GetLine(lineIndex)) || GetLine(lineIndex).StartsWith("#"))
				{
					lineIndex++;
					continue;
				}
				var list = GetBlockLines(ref lineIndex);
				bool add = false;
				var firstLine = list[0];
				list.RemoveAt(0);
				var block = list.ToArray();

				if (firstLine.StartsWith("ADD "))
				{
					add = true;
					firstLine = firstLine.Substring(4).TrimStart();
				}

				var type = ReflectionTest.GetInstanceType(firstLine);
				var identifier = firstLine.Split(' ')[0];
				if (type == ReflectionTest.AttributeTypeInfo.Material)
				{
					var mat = ReflectionTest.CreateMaterial(identifier, block);
					GetName(firstLine, out var name);
					materials.Add(name, mat);
				}
				else if (type == ReflectionTest.AttributeTypeInfo.Skybox)
				{
					//TODO: load skyboxes
				}
				else if (type == ReflectionTest.AttributeTypeInfo.SceneObject)
				{
					GetName(firstLine, out var name);
					var so = ReflectionTest.CreateSceneObject(this, identifier, name, block);
					if (add) sn.AddObject(so);
					if (name != null)
					{
						objectList.Add(name, so);
					}
				}
			}
			fileContents = lines;
			return sn;
		}

		private void GetName(string firstLine, out string name)
		{
			name = null;
			var split = firstLine.Split(' ');
			if (split.Length > 1)
			{
				name = split[1];
			}
		}

		private List<string> GetBlockLines(ref int i)
		{
			List<string> list = new List<string>();
			list.Add(fileContents[i].Trim());
			int indent = GetIndent(fileContents[i]) + 1;
			i++;
			while (GetIndent(GetLine(i)) >= indent)
			{
				string ln = GetLine(i);
				i++;
				ln = ln.Trim();
				if (ln.StartsWith("#")) continue;
				list.Add(ln);
			}
			return list;
		}

		private int GetIndent(string s)
		{
			if (string.IsNullOrWhiteSpace(s)) return -1;
			return s.Length - s.TrimStart('\t').Length;
		}

		private string GetLine(int i)
		{
			if (i < fileContents.Length)
			{
				return fileContents[i];
			}
			else
			{
				return null;
			}
		}
	}
}
