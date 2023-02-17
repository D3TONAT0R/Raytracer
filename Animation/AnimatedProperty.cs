using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	public class AnimatedProperty
	{

		public struct Keyframe
		{
			public float time;

			public Vector4? values;
			public float easingIn;
			public float easingOut;

			public Keyframe(float t)
			{
				time = t;
				values = null;
				easingIn = 0;
				easingOut = 0;
			}

			public Keyframe(float t, float? v) : this(t)
			{
				if(v.HasValue) values = new Vector4(v.Value, 0, 0, 0);
				else values = null;
			}

			public Keyframe(float t, Vector2? v) : this(t)
			{
				if(v.HasValue) values = new Vector4(v.Value, 0, 0);
				else values = null;
			}

			public Keyframe(float t, Vector3? v) : this(t)
			{
				if(v.HasValue) values = new Vector4(v.Value, 0);
				else values = null;
			}

			public Keyframe(float t, Vector4? v) : this(t)
			{
				values = v;
			}

			public static Keyframe Parse(string timestamp, string data)
			{
				var split = data.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
				float time = float.Parse(timestamp);
				if(split[0] != "*")
				{
					float[] vector = new float[4];
					for(int i = 0; i < split.Length; i++)
					{
						vector[i] = float.Parse(split[i]);
					}
					return new Keyframe(time, new Vector4(vector[0], vector[1], vector[2], vector[3]));
				}
				else
				{
					return new Keyframe(time);
				}
			}
		}

		public enum AnimatedFieldType
		{
			Float,
			Vector2,
			Vector3,
			Vector4,
			Color
		}

		private string targetObjectPath;
		private string targetPropertyIdentifier;

		public SceneObject targetObject;
		private FieldInfo fieldInfo;
		private object defaultValue;
		public AnimatedFieldType fieldType;

		public List<Keyframe> keyframes = new List<Keyframe>();

		public float StartTime => keyframes.Count > 0 ? keyframes[0].time : 0;
		public float EndTime => keyframes.Count > 0 ? keyframes[keyframes.Count - 1].time : 0;
		public float Duration => EndTime - StartTime;

		public AnimatedProperty(SceneObject target, string property, params Keyframe[] keyframes)
		{
			targetObject = target;
			targetPropertyIdentifier = property;
			Init(null);
			this.keyframes.AddRange(keyframes);
		}

		public AnimatedProperty(string targetObjectPath, string targetPropertyIdentifier)
		{
			this.targetObjectPath = targetObjectPath;
			this.targetPropertyIdentifier = targetPropertyIdentifier;
		}

		public void Init(Scene scene)
		{
			if(targetObject == null && scene != null) {
				if(targetObjectPath == "<CAMERA>")
				{
					targetObject = Camera.MainCamera;
				}
				else
				{
					targetObject = scene.FindSceneObject(targetObjectPath);
				}
			}
			if(fieldInfo == null)
			{
				fieldInfo = Reflector.GetExposedField(targetObject.GetType(), targetPropertyIdentifier);
			}
			defaultValue = fieldInfo.GetValue(targetObject);
			fieldType = EvaluateFieldType();
		}

		private AnimatedFieldType EvaluateFieldType()
		{
			var t = fieldInfo.FieldType;
			if(t == typeof(float)) return AnimatedFieldType.Float;
			else if(t == typeof(Vector2)) return AnimatedFieldType.Vector2;
			else if(t == typeof(Vector3)) return AnimatedFieldType.Vector3;
			else if(t == typeof(Vector4)) return AnimatedFieldType.Vector4;
			else if(t == typeof(Color)) return AnimatedFieldType.Color;
			else throw new InvalidOperationException();
		}

		public void SampleAnimation(float time)
		{
			fieldInfo.SetValue(targetObject, Evaluate(time, defaultValue));
		}

		private object Evaluate(float time, object defaultValue)
		{
			int index = GetLowerIndex(time, out var lerp);
			var prev = keyframes[index];
			var next = keyframes[index + 1];

			//Apply easing
			float x = lerp;
			float p0 = Lerp(0, x, x * next.easingIn);
			float x1 = 1 - x;
			float p1 = Lerp(0, x1, x1 * -next.easingOut) + 1f;
			lerp = Lerp(p0, p1, x);

			var packedDefault = PackValues(defaultValue);
			var i = Lerp(prev.values, keyframes[index + 1].values, lerp, packedDefault);
			switch(fieldType)
			{
				case AnimatedFieldType.Float: return i.X;
				case AnimatedFieldType.Vector2: return new Vector2(i.X, i.Y);
				case AnimatedFieldType.Vector3: return new Vector3(i.X, i.Y, i.Z);
				case AnimatedFieldType.Vector4: return i;
				case AnimatedFieldType.Color: return new Color(i.X, i.Y, i.Z, i.W);
				default: throw new InvalidOperationException();
			}
		}

		private int GetLowerIndex(float t, out float lerp)
		{
			if(t <= keyframes.First().time)
			{
				lerp = 0;
				return 0;
			}
			else if(t >= keyframes.Last().time)
			{
				lerp = 1;
				return keyframes.Count - 2;
			}

			int i = 0;
			while(t > keyframes[i + 1].time)
			{
				i++;
			}

			float a = keyframes[i].time;
			float b = keyframes[i + 1].time;

			lerp = (t - a) / (b - a);
			return i;
		}

		public static Vector4 PackValues(object v)
		{
			switch(v)
			{
				case float v1:
					return new Vector4(v1, 0, 0, 0);
				case Vector2 v2:
					return new Vector4(v2, 0, 0);
				case Vector3 v3:
					return new Vector4(v3, 0);
				case Vector4 v4:
					return v4;
				case Color c:
					return new Vector4(c.r, c.g, c.b, c.a);
				default:
					throw new NotSupportedException();
			}
		}

		private static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		public static Vector4 Lerp(Vector4? a, Vector4? b, float t, Vector4 defaultValue)
		{
			if(a == null) a = defaultValue;
			if(b == null) b = defaultValue;
			return Vector4.Lerp(a.Value, b.Value, t);
		}

		public override string ToString()
		{
			return $"({targetObjectPath ?? targetObject?.name ?? "<NULL>"}:{targetPropertyIdentifier ?? "<NULL>"} [{StartTime:D2} - {EndTime}]";
		}
	}
}
