﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer
{
	public class Scene
	{

		public string sceneName;
		public string rootDirectory;

		public Environment environment;

		public Dictionary<string, Color> globalColors = new Dictionary<string, Color>();
		public Dictionary<string, Material> globalMaterials = new Dictionary<string, Material>();

		public List<SceneObject> prefabContent = new List<SceneObject>();
		public List<SceneObject> sceneContent = new List<SceneObject>();
		public bool hasContentUpdate = true;
		AABB worldAABB = new AABB(-Vector3.One * 1000, Vector3.One * 1000);
		AABB sceneObjectsAABB = new AABB();

		List<Shape> shapes = new List<Shape>();
		List<Light> lights = new List<Light>();

		public SceneObject remoteControlledObject;

		public Scene(string name)
		{
			sceneName = name;
			environment = new Environment();
		}

		public void AddObject(SceneObject obj)
		{
			sceneContent.Add(obj);
		}

		public void AddObjects(params SceneObject[] objects)
		{
			foreach(var obj in objects)
			{
				AddObject(obj);
			}
		}

		public void AddDefaultDirectionalLight()
		{
			AddObject(Light.CreateDirectionalLight(Vector3.Normalize(new Vector3(-1, -1, 0.7f)), 1.6f, new Color(1, 0.9f, 0.75f)));
		}

		public void OnBeginRender(float expansionAmount)
		{
			shapes.Clear();
			lights.Clear();
			foreach(var obj in sceneContent)
			{
				RegisterSceneContent(obj);
			}
			//shapeAABBs.Clear();
			sceneObjectsAABB = AABB.Empty;
			foreach(var s in sceneContent)
			{
				s.SetupForRendering();
				sceneObjectsAABB = sceneObjectsAABB.Join(s.GetTotalShapeAABB());
			}
			/*
			foreach(var s1 in shapes)
			{
				foreach(var s in s1.GetSubShapes())
				{
					s.SetupForRendering();
					sceneObjectsAABB = sceneObjectsAABB.Join(s.ShapeAABB.Offset(s.HierarchyPositionOffset));
					//s.SetupAABBs(expandedAABBs, expansionAmount);
					//shapeAABBs.Add(s, new AABB[] { s.ShapeAABB, s.ShapeAABB.Expand(expansionAmount) });
				}
			}
			*/
		}

		void RegisterSceneContent(SceneObject obj)
		{
			if(!obj.IsInitialized)
			{
				obj.Initialize();
			}
			shapes.AddRange(obj.GetContainedObjectsOfType<Shape>());
			lights.AddRange(obj.GetContainedObjectsOfType<Light>());
			/*
			if(obj is ObjectInstance instance)
			{
				RegisterSceneContent(instance.referencedObject);
			}
			else if(obj is Group group)
			{
				shapes.AddRange(group.GetContainedObjectsOfType<Shape>());
				lights.AddRange(group.GetContainedObjectsOfType<Light>());
			}
			else if(obj is Shape)
			{
				shapes.Add(obj as Shape);
			}
			else if(obj is Light)
			{
				lights.Add(obj as Light);
			}
			*/
		}

		static Random r = new Random();

		public List<Shape> GetIntersectingShapes(Ray ray)
		{
			var list = new List<Shape>();
			foreach(var o in sceneContent)
			{
				if(o.CanContainShapes)
				{
					list.AddRange(o.GetIntersectingShapes(ray));
				}
			}
			/*
			foreach(var s in shapes)
			{
				//TODO: correct?
				//if(s.ShapeAABB.Offset(s.HierarchyPositionOffset).Intersects(ray)) list.Add(s);
				if(s.ShapeAABB.Intersects(ray)) list.Add(s);
			}
			*/
			return list;
		}

		public List<Light> GetContributingLights(Vector3 pos)
		{
			var list = new List<Light>();
			for(int i = 0; i < lights.Count; i++)
			{
				if(lights[i].Contributes(pos)) list.Add(lights[i]);
			}
			return list;
		}

		public bool IsInWorldBounds(Vector3 pos)
		{
			return sceneObjectsAABB.IsInside(pos);
			//return worldAABB.IsInside(pos);
		}

		public Shape[] GetAABBIntersectingShape(Vector3 pos)
		{
			return GetAABBIntersectingShapes(pos, shapes);
		}

		public Shape[] GetAABBIntersectingShapes(Vector3 pos, List<Shape> query)
		{
			List<Shape> list = new List<Shape>();
			if(sceneObjectsAABB.IsInside(pos))
			{
				foreach(var s in query)
				{
					if(s.ExpandedAABB.IsInside(pos)) list.Add(s);
				}
			}
			return list.ToArray();
		}

		public SceneObject GetSceneObject(string name)
		{
			return sceneContent.FirstOrDefault((c) => c.name == name);
		}

		public SceneObject GetPrefabObject(string name)
		{
			return prefabContent.FirstOrDefault((c) => c.name == name);
		}

		public SceneObject GetPrefabOrSceneObject(string name)
		{
			return GetPrefabObject(name) ?? GetSceneObject(name);
		}
	}
}
