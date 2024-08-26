using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Raytracer
{
	public class Scene
	{

		public string sceneName;
		public string rootDirectory;
		public string sourceFile;
		public string SourceFileName => Path.GetFileNameWithoutExtension(sourceFile);

		public Environment environment;

		public Material DefaultMaterial = new Material()
		{
			shader = ShaderType.DefaultCheckered,
			mainColor = Color.White,
			secColor = Color.LightGray,
			reflectivity = 0f,
			textureTiling = new TilingVector(0, 0, 4, 4)
		};

		public Dictionary<string, Color> globalColors = new Dictionary<string, Color>();
		public Dictionary<string, Material> globalMaterials = new Dictionary<string, Material>();

		public List<CameraConfiguration> cameraConfigurations = new List<CameraConfiguration>();

		public List<SceneObject> prefabContent = new List<SceneObject>();
		public List<SceneObject> sceneContent = new List<SceneObject>();
		public bool hasContentUpdate = true;
		AABB sceneObjectsAABB = new AABB();

		List<Shape> shapes = new List<Shape>();
		List<Light> lights = new List<Light>();

		public SceneObject remoteControlledObject;

		public Animator animator;

		public Scene(string name, string sourceFilePath)
		{
			sceneName = name;
			environment = new Environment();
			sourceFile = sourceFilePath;
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

		public void Initialize()
		{
			foreach(var obj in sceneContent)
			{
				if(!obj.IsInitialized)
				{
					obj.Initialize(this);
				}
			}
			if(animator != null) animator.Init(this);
		}

		public void OnBeginRender()
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
				s.SetupMatrix();
				s.SetupForRendering();
				sceneObjectsAABB = sceneObjectsAABB.Join(s.GetTotalShapeAABB());
			}
			sceneObjectsAABB = sceneObjectsAABB.Join(new AABB(Camera.MainCamera.ActualCameraPosition, Camera.MainCamera.ActualCameraPosition));
			sceneObjectsAABB = sceneObjectsAABB.Expand(2.0f);
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

		public List<Shape> GetIntersectingShapes(Ray ray, VisibilityFlags flags)
		{
			var list = new List<Shape>();
			foreach(var o in sceneContent)
			{
				if(o.CanContainShapes)
				{
					list.AddRange(o.GetIntersectingShapes(ray, flags));
				}
			}
			list.RemoveAll((s) => !s.VisibleInHierarchy);
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

		public Shape[] GetAABBIntersectingShapes(Vector3 pos, List<Shape> query)
		{
			List<Shape> list = new List<Shape>();
			if(sceneObjectsAABB.IsInside(pos))
			{
				foreach(var s in query)
				{
					if(s.ExpandedAABB.IsInside(s.WorldToLocalPoint(pos))) list.Add(s);
				}
			}
			return list.ToArray();
		}

		public SceneObject FindSceneObject(string path)
		{
			string name = path.Split('/')[0];
			var root = sceneContent.FirstOrDefault((c) => c.name == name);
			if(root != null)
			{
				if(path.Length > name.Length + 1)
				{
					return root.FindChildByPath(path.Substring(name.Length + 1));
				}
				else
				{
					return root;
				}
			}
			else
			{
				return null;
			}
		}

		public SceneObject GetPrefabObject(string name)
		{
			return prefabContent.FirstOrDefault((c) => c.name == name);
		}

		public SceneObject GetPrefabOrSceneObject(string name)
		{
			return GetPrefabObject(name) ?? FindSceneObject(name);
		}
	}
}
