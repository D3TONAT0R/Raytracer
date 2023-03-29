using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using static Raytracer.SceneFileLoader;

namespace Raytracer
{

	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ObjectIdentifierAttribute : Attribute
	{

		public string identifier;

		public ObjectIdentifierAttribute(string id)
		{
			identifier = id.ToUpper();
		}
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class DataIdentifierAttribute : Attribute
	{

		public string identifier;

		public float? numericIncrement = null;

		public DataIdentifierAttribute(string id)
		{
			identifier = id.ToUpper();
		}

		public DataIdentifierAttribute(string id, float increments) : this(id)
		{
			numericIncrement = increments;
		}
	}

	public static class Reflector
	{

		public class ExposedFieldSet
		{

			public class ExposedField
			{
				public DataIdentifierAttribute attribute;
				public string fieldName;
				public Type fieldType;

				public object GetValue(object target)
				{
					return target.GetType().GetField(fieldName).GetValue(target);
				}

				public void SetValue(object target, object value)
				{
					target.GetType().GetField(fieldName).SetValue(target, value);
				}
			}

			public Type type;
			public Dictionary<string, ExposedField> fields = new Dictionary<string, ExposedField>();

			public ExposedFieldSet(Type t)
			{
				if (t == null)
				{
					throw new NullReferenceException();
				}
				type = t;
			}
		}

		static Dictionary<string, ExposedFieldSet> exposedFieldSets;

		static Reflector()
		{
			exposedFieldSets = new Dictionary<string, ExposedFieldSet>();
			var attrs = Assembly.GetCallingAssembly().GetTypes()
								  .Where(m => m.GetCustomAttributes(typeof(ObjectIdentifierAttribute), false).Length > 0)
								  .ToArray();
			foreach (var a in attrs)
			{
				var typeInfo = new ExposedFieldSet(a);
				var fields = a.GetFields();
				foreach (var f in fields)
				{
					var attr = f.GetCustomAttribute<DataIdentifierAttribute>();
					if (attr != null)
					{
						var f2 = new ExposedFieldSet.ExposedField();
						f2.attribute = attr;
						f2.fieldName = f.Name;
						f2.fieldType = f.FieldType;
						typeInfo.fields.Add(attr.identifier, f2);
					}
				}
				exposedFieldSets.Add(a.GetCustomAttribute<ObjectIdentifierAttribute>().identifier, typeInfo);
			}
		}

		public static ExposedFieldSet GetExposedFieldSet(Type type)
		{
			var attr = type.GetCustomAttribute<ObjectIdentifierAttribute>();
			if (attr != null)
			{
				exposedFieldSets.TryGetValue(attr.identifier, out var value);
				return value;
			}
			else
			{
				return null;
			}
		}

		public static FieldInfo GetExposedField(Type type, string identifier)
		{
			foreach (var o in exposedFieldSets.Values)
			{
				if (o.type == type)
				{
					return type.GetField(o.fields[identifier].fieldName);
				}
			}
			throw new KeyNotFoundException();
		}

		internal static SceneObject CreateSceneObject(Scene scene, BlockContent block)
		{
			var fieldSet = exposedFieldSets[block.keyword.ToUpper()];
			SceneObject obj = (SceneObject)Activator.CreateInstance(fieldSet.type);
			obj.name = !string.IsNullOrWhiteSpace(block.name) ? block.name : null;
			if(obj is IReferencedObject iRef)
			{
				if(block.refName == null) throw new NullReferenceException("SceneObject is missing a required object reference.");
				iRef.ReferencedObjectName = block.refName;
			}
			foreach(var d in block.data)
			{
				bool found = false;
				foreach (var f in fieldSet.fields)
				{
					if (d.keyword == f.Key)
					{
						FieldInfo fi = fieldSet.type.GetField(f.Value.fieldName);
						fi.SetValue(obj, ParseData(scene, fi.FieldType, d));
						found = true;
						break;
					}
				}
				if(!found)
				{
					throw new InvalidOperationException($"Unknown identifier '{d.keyword}' in block '{block.keyword}:{(string.IsNullOrEmpty(block.name) ? block.name : "<Unnamed>")}'\nat line {d.lineNumber}.");
				}
			}
			return obj;
		}

		internal static CameraConfiguration CreateCameraConfiguration(Scene scene, BlockContent block)
		{
			var config = new CameraConfiguration(block.keyword);
			var fs = exposedFieldSets["CAMERA_CONFIGURATION"];
			foreach (var d in block.data)
			{
				foreach (var f in fs.fields)
				{
					if (d.keyword == f.Key)
					{
						FieldInfo fi = fs.type.GetField(f.Value.fieldName);
						fi.SetValue(config, ParseData(scene, fi.FieldType, d));
					}
				}
			}
			return config;
		}

		/*public static SceneObject CreateSceneObjectOld(SceneFileLoader loader, string identifier, string name, string[] data) {
			currentSceneLoader = loader;
			var t = objects[identifier];
			SceneObject obj = (SceneObject)Activator.CreateInstance(t.type);
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
		*/

		internal static Material CreateMaterial(BlockContent block, Scene scene)
		{
			var t = exposedFieldSets["MATERIAL"];
			Material mat = (Material)Activator.CreateInstance(t.type);
			foreach (var d in block.data)
			{
				foreach (var f in t.fields)
				{
					if (d.keyword == f.Key)
					{
						FieldInfo fi = t.type.GetField(f.Value.fieldName);
						fi.SetValue(mat, ParseData(scene, fi.FieldType, d));
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
			{ typeof(Color), (s) => Color.Parse(s) },
			{ typeof(TilingVector), (s) => TilingVector.Parse(s) },
			//{ typeof(Sampler2D), (s) => Sampler2D.Create(s) },
			//{ typeof(Material), (s) => currentSceneLoader.materials[s] },
			//{ typeof(SceneObject[]), (s) => GetSceneObjects(s.Split(' ')) },
			//{ typeof(SolidShape[]), (s) => GetSolids(s.Split(' ')) }
		};

		static object ParseData(Scene scene, Type targetType, Content data)
		{
			if(targetType == typeof(Material))
			{
				if (data is BlockContent bc)
				{
					return CreateMaterial(bc, scene);
				}
				else
				{
					return scene.globalMaterials[((StringContent)data).data];
				}
			}
			else if(targetType == typeof(Sampler2D))
			{
				if(data is StringContent sc)
				{
					return Sampler2D.Create(sc.data, scene.rootDirectory);
				}
				else
				{
					//This isn't a common use case, leave it for now
					throw new NotImplementedException();
				}
			}
			else if (targetType.IsAssignableFrom(typeof(SceneObject)))
			{
				if (data is BlockContent bc)
				{
					return CreateSceneObject(scene, bc);
				}
				else
				{
					//This isn't a common use case, leave it for now
					throw new NotImplementedException();
				}
			}
			else if(targetType == typeof(SolidShape[]))
			{
				var solids = new List<SolidShape>();
				foreach(var d in ((BlockContent)data).data)
				{
					var so = (SceneObject)ParseData(scene, typeof(SceneObject), d);
					if (!so.GetType().IsSubclassOf(typeof(SolidShape))) throw new InvalidCastException($"{so.GetType()} is not a SolidShape.");
					solids.Add((SolidShape)so);
				}
				return solids.ToArray();
			}
			else if(targetType == typeof(SceneObject[]))
			{
				var children = new List<SceneObject>();
				foreach(var d in ((BlockContent)data).data)
				{
					var so = (SceneObject)ParseData(scene, typeof(SceneObject), d);
					children.Add(so);
				}
				return children.ToArray();
			}
			else if(targetType == typeof(Color))
			{
				var sc = (StringContent)data;
				if(char.IsLetter(sc.data[0]))
				{
					return new Color(scene, sc.data);
				}
				else
				{
					return Color.Parse(sc.data);
				}
			}
			else if (targetType.IsEnum)
			{
				return Enum.Parse(targetType, ((StringContent)data).data, true);
			}
			else if(targetType == typeof(Gradient))
			{
				Gradient grad = new Gradient();
				foreach(var d in ((BlockContent)data).data)
				{
					var key = (StringContent)d;
					grad.keys.Add(new GradientKey(float.Parse(key.keyword), Color.Parse(key.data)));
				}
				return grad;
			}
			else if (parsingTable.ContainsKey(targetType))
			{
				return parsingTable[targetType](((StringContent)data).data);
			}
			else
			{
				return null;
			}
		}

		public static Vector3 ParseVector3(string s)
		{
			Vector3 vec = new Vector3();
			var comps = s.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
			vec.X = float.Parse(comps[0]);
			vec.Y = float.Parse(comps[1]);
			vec.Z = float.Parse(comps[2]);
			return vec;
		}

		public static void TransferFieldData(SceneObject from, SceneObject to)
		{
			var fromType = from.GetType();
			var toType = to.GetType();
			var fields = fromType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach(var f in fields)
			{
				var v = f.GetValue(from);
				var targetField = toType.GetField(f.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if(targetField != null)
				{
					targetField.SetValue(to, v);
				}
			}
		}

		internal static void LoadData<T>(T target, BlockContent dataBlock, Scene parentScene)
		{
			var members = target.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(m => m.GetCustomAttribute<DataIdentifierAttribute>(true) != null)
				.ToDictionary(m => m.GetCustomAttribute<DataIdentifierAttribute>().identifier, m => m);
			foreach(var data in dataBlock.data)
			{
				if(members.TryGetValue(data.keyword, out var m))
				{
					SetMemberValue(target, m, ParseData(parentScene, GetMemberType(m), data));
				}
				else
				{
					throw new InvalidOperationException($"Unknown identifier '{data.keyword}' in block '{dataBlock.keyword}'\nat line {data.lineNumber}.");
				}
			}
		}

		internal static void SetMemberValue(object obj, MemberInfo m, object value)
		{
			if(m is FieldInfo fi) fi.SetValue(obj, value);
			else if(m is PropertyInfo pi) pi.SetValue(obj, value);
			else throw new InvalidOperationException();
		}

		internal static Type GetMemberType(MemberInfo m)
		{
			if(m is FieldInfo fi) return fi.FieldType;
			else if(m is PropertyInfo pi) return pi.PropertyType;
			else throw new InvalidOperationException();
		}

		public static SceneObject CloneObject(SceneObject obj)
		{
			var clone = (SceneObject)Activator.CreateInstance(obj.GetType());
			TransferFieldData(obj, clone);
			return clone;
		}
	}
}
