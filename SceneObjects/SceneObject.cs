using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Raytracer
{
	[StructLayout(LayoutKind.Sequential)]
	public abstract class SceneObject
	{
		public bool IsInitialized { get; private set; }
		public SceneObject parent;
		public string name;
		[DataIdentifier("VISIBLE")]
		public bool visible = true;
		[DataIdentifier("POSITION", 0.1f)]
		public Vector3 localPosition = Vector3.Zero;
		[DataIdentifier("ROTATION", 5.0f)]
		public Vector3 localRotation = Vector3.Zero;
		[DataIdentifier("SCALE", 0.1f)]
		public Vector3 localScale = Vector3.One;

		private Matrix4x4 childMatrix = Matrix4x4.Identity;
		public Matrix4x4 LocalToWorldMatrix { get; private set; } = Matrix4x4.Identity;
		public Matrix4x4 WorldToLocalMatrix { get; private set; } = Matrix4x4.Identity;

		public bool VisibleInHierarchy => visible && (parent?.VisibleInHierarchy ?? true);

		public Vector3 HierarchyPositionOffset
		{
			get
			{
				if(parent == null)
				{
					return Vector3.Zero;
				}
				else
				{
					return parent.HierarchyPositionOffset + parent.localPosition;
				}
			}
		}
		public Vector3 WorldPosition
		{
			get
			{
				return HierarchyPositionOffset + localPosition;
			}
		}

		public virtual Material OverrideMaterial => parent?.OverrideMaterial;

		public virtual bool CanContainShapes => false;

		public SceneObject() { }

		public SceneObject(string name)
		{
			this.name = name;
		}

		public void Initialize(Scene parentScene)
		{
			OnInit(parentScene);
			IsInitialized = true;
		}

		protected virtual void OnInit(Scene parentScene)
		{

		}

		public virtual IEnumerable<T> GetContainedObjectsOfType<T>() where T : SceneObject
		{
			if(typeof(T).IsAssignableFrom(GetType())) yield return this as T;
		}

		public virtual SceneObject FindChildByName(string name)
		{
			foreach(var child in GetContainedObjectsOfType<SceneObject>())
			{
				if(child.name == name)
				{
					return child;
				}
			}
			return null;
		}

		public virtual SceneObject FindChildByPath(string path)
		{
			string name;
			if(path.Contains("/")) {
				name = path.Substring(0, path.IndexOf('/'));
			}
			else
			{
				name = path;
			}
			var so = FindChildByName(name);
			if(name.Length == path.Length)
			{
				return so;
			}
			else if(so != null)
			{
				return so.FindChildByPath(path.Substring(name.Length + 1));
			}
			else
			{
				return null;
			}
		}

		public override string ToString()
		{
			return $"[{name} ({GetType().Name.ToUpper()})]";
		}

		public virtual SceneObject Clone()
		{
			var copy = Reflector.CloneObject(this);
			copy.Uninitialize();
			return copy;
		}

		public void Uninitialize()
		{
			IsInitialized = false;
		}

		public virtual void SetupMatrix()
		{
			const float deg2rad = (float)Math.PI / 180f;
			childMatrix = Matrix4x4.CreateScale(localScale) * Matrix4x4.CreateFromYawPitchRoll(localRotation.Y * deg2rad, localRotation.X * deg2rad, localRotation.Z * deg2rad) * Matrix4x4.CreateTranslation(localPosition);
			LocalToWorldMatrix = GetWorldMatrix();
			Matrix4x4 inv;
			Matrix4x4.Invert(LocalToWorldMatrix, out inv);
			WorldToLocalMatrix = inv;
			foreach(var c in GetContainedObjectsOfType<SceneObject>())
			{
				if(c == this) continue;
				if(c != null) c.SetupMatrix();
			}
		}

		public abstract void SetupForRendering();

		public virtual AABB GetTotalShapeAABB() => AABB.Empty;

		public virtual IEnumerable<Shape> GetIntersectingShapes(Ray ray)
		{
			yield break;
		}

		public virtual IEnumerable<Shape> GetIntersectingShapes(Ray ray, VisibilityFlags flags)
		{
			foreach(var s in GetIntersectingShapes(ray))
			{
				if(s.visibilityFlags.HasFlag(flags))
				{
					yield return s;
				}
			}
		}

		private Matrix4x4 GetWorldMatrix()
		{
			if(parent != null) return childMatrix * parent.GetWorldMatrix();
			else return childMatrix;
		}

		public Vector3 LocalToWorldPoint(Vector3 local) => Vector3.Transform(local, LocalToWorldMatrix);
		public Vector3 WorldToLocalPoint(Vector3 world) => Vector3.Transform(world, WorldToLocalMatrix);
		public Vector3 LocalToWorldNormal(Vector3 localNormal) => Vector3.TransformNormal(localNormal, LocalToWorldMatrix);
		public Vector3 WorldToLocalNormal(Vector3 worldNormal) => Vector3.TransformNormal(worldNormal, WorldToLocalMatrix);
	}
}
