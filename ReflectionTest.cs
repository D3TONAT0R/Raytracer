using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {

	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ObjectIdentifierAttribute : Attribute {

		public string identifier;

		public ObjectIdentifierAttribute(string id) {
			identifier = id.ToUpper();
		}
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class DataIdentifierAttribute : Attribute {

		public string identifier;

		public DataIdentifierAttribute(string id) {
			identifier = id.ToUpper();
		}
	}

	public static class ReflectionTest {

		public enum AttributeTypeInfo {
			Unknown,
			SceneObject,
			Material,
			Data
		}

		public class ExposedFieldSet {

			public class ExposedField {
				public DataIdentifierAttribute attribute;
				public string fieldName;
				public Type fieldType;

				public object GetValue(object target) {
					return target.GetType().GetField(fieldName).GetValue(target);
				}

				public void SetValue(object target, object value) {
					target.GetType().GetField(fieldName).SetValue(target, value);
				}
			}

			public Type type;
			public Dictionary<string, ExposedField> fields = new Dictionary<string, ExposedField>();

			public ExposedFieldSet(Type t) {
				if(t == null) {
					throw new NullReferenceException();
				}
				type = t;
			}
		}

		static Dictionary<string, ExposedFieldSet> objects;

		static ReflectionTest() {
			objects = new Dictionary<string, ExposedFieldSet>();
			var attrs = Assembly.GetCallingAssembly().GetTypes()
								  .Where(m => m.GetCustomAttributes(typeof(ObjectIdentifierAttribute), false).Length > 0)
								  .ToArray();
			foreach(var a in attrs) {
				var typeInfo = new ExposedFieldSet(a);
				var fields = a.GetFields();
				foreach(var f in fields) {
					var attr = f.GetCustomAttribute<DataIdentifierAttribute>();
					if(attr != null) {
						var f2 = new ExposedFieldSet.ExposedField();
						f2.attribute = attr;
						f2.fieldName = f.Name;
						f2.fieldType = f.FieldType;
						typeInfo.fields.Add(attr.identifier, f2);
					}
				}
				objects.Add(a.GetCustomAttribute<ObjectIdentifierAttribute>().identifier, typeInfo);
			}
		}

		public static ExposedFieldSet GetExposedFieldSet(Type type) {
			var attr = type.GetCustomAttribute<ObjectIdentifierAttribute>();
			if(attr != null) {
				objects.TryGetValue(attr.identifier, out var value);
				return value;
			} else {
				return null;
			}
		}

		public static AttributeTypeInfo GetInstanceType(string identifier) {
			identifier = identifier.Split(' ')[0].Split('_')[0].Trim();
			foreach(var a in objects) {
				if(a.Key == identifier) {
					var t = a.Value.type;
					if(t.IsSubclassOf(typeof(SceneObject))) {
						return AttributeTypeInfo.SceneObject;
					} else if(t == typeof(Material) || t.IsSubclassOf(typeof(Material))) {
						return AttributeTypeInfo.Material;
					}
				}
			}
			return AttributeTypeInfo.Unknown;
		}

		public static FieldInfo GetField(Type type, string identifier) {
			foreach(var o in objects.Values) {
				if(o.type == type) {
					return type.GetField(o.fields[identifier].fieldName);
				}
			}
			throw new KeyNotFoundException();
		}

		static SceneFileLoader currentSceneLoader;

		public static SceneObject CreateSceneObject(SceneFileLoader loader, string identifier, string name, string[] data) {
			currentSceneLoader = loader;
			string[] split = identifier.Split('_');
			identifier = split[0];
			var t = objects[identifier];
			SceneObject obj = (SceneObject)Activator.CreateInstance(t.type);
			if(split.Length > 1) obj.HandleExtraIdentifier(split[1]);
			obj.identifier = name;
			for(int i = 0; i < data.Length; i++) {
				string d = data[i];
				foreach(var f in t.fields) {
					if(d.StartsWith(f.Key)) {
						d = d.Substring(f.Key.Length).Trim();
						FieldInfo fi = t.type.GetField(f.Value.fieldName);
						fi.SetValue(obj, ParseData(fi.FieldType, d));
					}
				}				
			}
			return obj;
		}

		public static Material CreateMaterial(string identifier, string[] data) {
			string[] split = identifier.Split('_');
			identifier = split[0];
			var t = objects[identifier];
			Material mat = (Material)Activator.CreateInstance(t.type);
			if(split.Length > 1) mat.HandleExtraIdentifier(split[1]);
			for(int i = 0; i < data.Length; i++) {
				string d = data[i];
				foreach(var f in t.fields) {
					if(d.StartsWith(f.Key)) {
						d = d.Substring(f.Key.Length).Trim();
						FieldInfo fi = t.type.GetField(f.Value.fieldName);
						fi.SetValue(mat, ParseData(fi.FieldType, d));
					}
				}
			}
			return mat;
		}

		static Dictionary<Type, Func<string, object>> parsingTable = new Dictionary<Type, Func<string, object>>() {
			{ typeof(int), (s) => int.Parse(s) },
			{ typeof(float), (s) => float.Parse(s) },
			{ typeof(string), (s) => s },
			{ typeof(Vector3), (s) => ParseVector3(s) },
			{ typeof(Color), (s) => ParseColor(s) },
			{ typeof(TilingVector), (s) => ParseTiling(s) },
			{ typeof(Sampler2D), (s) => Sampler2D.Create(s) },
			{ typeof(Material), (s) => currentSceneLoader.materials[s] },
			{ typeof(SceneObject[]), (s) => GetSceneObjects(s.Split(' ')) },
			{ typeof(SolidShape[]), (s) => GetSolids(s.Split(' ')) }
		};

		static object ParseData(Type targetType, string data) {
			if(targetType.IsEnum) {
				return Enum.Parse(targetType, data);
			} else if(parsingTable.ContainsKey(targetType)) {
				return parsingTable[targetType](data);
			} else {
				return null;
			}
		}

		public static Vector3 ParseVector3(string s) {
			Vector3 vec = new Vector3();
			var comps = s.Split(' ');
			vec.X = float.Parse(comps[0]);
			vec.Y = float.Parse(comps[1]);
			vec.Z = float.Parse(comps[2]);
			return vec;
		}

		public static Color ParseColor(string s) {
			Color col = new Color();
			var comps = s.Split(' ');
			float[] vals = new float[comps.Length];
			for(int i = 0; i < comps.Length; i++) {
				vals[i] = float.Parse(comps[i]);
			}
			if(comps.Length == 1) {
				col.r = vals[0];
				col.g = vals[0];
				col.b = vals[0];
			} else if(comps.Length >= 3) {
				col.r = vals[0];
				col.g = vals[1];
				col.b = vals[2];
				if(comps.Length >= 4) {
					col.a = vals[3];
				} else {
					col.a = 1;
				}
			}
			return col;
		}

		public static TilingVector ParseTiling(string s) {
			var comps = s.Split(' ');
			var tv = new TilingVector();
			for(int i = 0; i < comps.Length; i++) {
				if(i == 0) tv.x = float.Parse(comps[i]);
				else if(i == 1) tv.y = float.Parse(comps[i]);
				else if(i == 2) tv.width = float.Parse(comps[i]);
				else if(i == 3) tv.height = float.Parse(comps[i]);
				else if(i == 4) tv.angle = float.Parse(comps[i]);
			}
			return tv;
		}

		public static SceneObject[] GetSceneObjects(string[] names) {
			SceneObject[] arr = new SceneObject[names.Length];
			for(int i = 0; i < names.Length; i++) {
				arr[i] = currentSceneLoader.objectList[names[i]];
			}
			return arr;
		}

		public static SceneObject[] GetSolids(string[] names) {
			SolidShape[] arr = new SolidShape[names.Length];
			for(int i = 0; i < names.Length; i++) {
				arr[i] = currentSceneLoader.objectList[names[i]] as SolidShape;
			}
			return arr;
		}
	}
}
