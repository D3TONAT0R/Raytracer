using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class AnimatedProperty {

		public class Keyframe {
			public float time;

			public float floatValue;
			public Vector3 vector3Value;

			public Keyframe(float t, float v) {
				time = t;
				floatValue = v;
			}

			public Keyframe(float t, Vector3 v) {
				time = t;
				vector3Value = v;
			}
		}

		public enum AnimatedFieldType {
			Float,
			Vector3
		}

		public SceneObject targetObject;
		public string targetField;
		private FieldInfo fieldInfo;
		public AnimatedFieldType fieldType;

		public List<Keyframe> keyframes;

		public float Length => keyframes.Last().time;

		public AnimatedProperty(SceneObject target, string property, params Keyframe[] keyframes) {
			targetObject = target;
			targetField = property;
			fieldInfo = Reflector.GetField(target.GetType(), property);
			if(fieldInfo.FieldType == typeof(float)) {
				fieldType = AnimatedFieldType.Float;
			} else if(fieldInfo.FieldType == typeof(Vector3)) {
				fieldType = AnimatedFieldType.Vector3;
			}
			this.keyframes = new List<Keyframe>();
			this.keyframes.AddRange(keyframes);
		}

		public void SampleAnimation(float time) {
			fieldInfo.SetValue(targetObject, Evaluate(time));
		}

		private object Evaluate(float time) {
			int i = GetLowerIndex(time, out var lerp);
			if(fieldType == AnimatedFieldType.Float) {
				return MathUtils.Lerp(keyframes[i].floatValue, keyframes[i + 1].floatValue, lerp);
			} else {
				return Vector3.Lerp(keyframes[i].vector3Value, keyframes[i + 1].vector3Value, lerp);
			}
		}

		private int GetLowerIndex(float t, out float lerp) {
			if(t <= keyframes.First().time) {
				lerp = 0;
				return 0;
			} else if(t >= keyframes.Last().time) {
				lerp = 1;
				return keyframes.Count - 2;
			}

			int i = 0;
			while(t > keyframes[i+1].time) {
				i++;
			}

			float a = keyframes[i].time;
			float b = keyframes[i + 1].time;

			lerp = (t-a) / (b-a);
			return i;
		}
	}
}
