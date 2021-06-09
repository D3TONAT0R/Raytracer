using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Raytracer {
	public abstract class SceneObject {

		public bool IsInitialized { get; private set; }
		public SceneObject parent;
		public string identifier;
		[DataIdentifier("VIS")]
		public bool visible = true;
		[DataIdentifier("POSITION")]
		public Vector3 localPosition;

		public Vector3 HierarchyPositionOffset {
			get {
				if(parent == null) {
					return Vector3.Zero;
				} else {
					return parent.localPosition;
				}
			}
		}

		public Vector3 WorldPosition {
			get {
				return HierarchyPositionOffset + localPosition;
			}
		}

		public SceneObject() { }

		public SceneObject(string name) {
			identifier = name;
		}

		public virtual void HandleExtraIdentifier(string extra) {

		}

		public void Initialize() {
			OnInit();
			IsInitialized = true;
		}
		protected virtual void OnInit() {

		}

		public override string ToString() {
			return $"[{identifier} ({GetType().Name.ToUpper()})]";
		}
	}
}
