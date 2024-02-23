using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;

namespace Raytracer
{
	public abstract class SceneObject
	{
		public bool IsInitialized { get; private set; }
		public SceneObject parent;
		public string name;
		[DataIdentifier("VISIBLE")]
		public bool visible = true;
		[DataIdentifier("POSITION")]
		public Vector3 localPosition;

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

		public abstract void SetupForRendering();

		public virtual AABB GetTotalShapeAABB() => AABB.Empty;

		public virtual IEnumerable<Shape> GetIntersectingShapes(Ray ray)
		{
			yield break;
		}

		public IEnumerable<Shape> GetIntersectingShapes(Ray ray, VisibilityFlags flags)
		{
			foreach(var s in GetIntersectingShapes(ray))
			{
				if(s.visibilityFlags.HasFlag(flags))
				{
					yield return s;
				}
			}
		}
	}
}
