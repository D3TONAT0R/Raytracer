using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class Scene {

		public string sceneName;

		public Gradient skyboxGradient;
		public Sampler2D skyboxTexture;
		public bool skyboxIsSpherical;

		public Color ambientColor = new Color(0.2f, 0.2f, 0.25f);
		public Color simpleSunColor = new Color(1.00f, 0.96f, 0.88f);

		public float? fogDistance = 250f;
		public Color fogColor = System.Drawing.Color.LightSkyBlue;

		public List<SceneObject> sceneContent = new List<SceneObject>();
		public bool hasContentUpdate = true;
		AABB worldAABB = new AABB(-Vector3.One * 20, Vector3.One * 20);
		AABB sceneObjectsAABB = new AABB();

		List<Shape> shapes = new List<Shape>();
		List<Light> lights = new List<Light>();

		public SceneObject remoteControlledObject;

		//public Dictionary<Shape, AABB[]> shapeAABBs = new Dictionary<Shape, AABB[]>();

		public Scene(string name) {
			sceneName = name;
			skyboxGradient = new Gradient(
				(0, System.Drawing.Color.Black),
				(0.49f, System.Drawing.Color.DarkOliveGreen),
				(0.51f, System.Drawing.Color.LightBlue),
				(1f, System.Drawing.Color.White));
			skyboxTexture = Sampler2D.Create("skybox");
			if(skyboxTexture == null) {
				skyboxTexture = Sampler2D.Create("skydome");
				if(skyboxTexture != null) skyboxIsSpherical = true;
			}
			/*skybox = new Gradient(
				(0, System.Drawing.Color.Red),
				(0.3999f, System.Drawing.Color.Red),
				(0.4f, System.Drawing.Color.Green),
				(0.5999f, System.Drawing.Color.Green),
				(0.6f, System.Drawing.Color.Blue),
				(1f, System.Drawing.Color.Blue));*/
		}

		public void AddObject(SceneObject obj) {
			sceneContent.Add(obj);
		}

		public void AddObjects(params SceneObject[] objects) {
			foreach(var obj in objects) {
				AddObject(obj);
			}
		}

		public void AddDefaultDirectionalLight() {
			AddObject(Light.CreateDirectionalLight(Vector3.Normalize(new Vector3(-1, -1, 0.7f)), 1.6f, new Color(1, 0.9f, 0.75f)));
		}

		public void OnBeginRender(float expansionAmount) {
			shapes.Clear();
			lights.Clear();
			foreach(var obj in sceneContent) {
				if(!obj.IsInitialized) {
					obj.Initialize();
				}
				if(obj is Group group) {
					shapes.AddRange(group.GetShapes());
					lights.AddRange(group.GetLights());
				} else if(obj is Shape) {
					shapes.Add(obj as Shape);
				} else if(obj is Light) {
					lights.Add(obj as Light);
				}
			}
			//shapeAABBs.Clear();
			sceneObjectsAABB = new AABB();
			foreach(var s1 in shapes) {
				foreach(var s in s1.GetSubShapes()) {
					s.SetupAABBs();
					sceneObjectsAABB = sceneObjectsAABB.Join(s.ShapeAABB.Offset(s.HierarchyPositionOffset));
					//s.SetupAABBs(expandedAABBs, expansionAmount);
					//shapeAABBs.Add(s, new AABB[] { s.ShapeAABB, s.ShapeAABB.Expand(expansionAmount) });
				}
			}
			foreach(var s in shapes) {
				s.OnBeginRender();
			}
		}

		public List<Shape> GetIntersectingShapes(Ray ray) {
			var list = new List<Shape>();
			foreach(var s in shapes) {
				if(s.ShapeAABB.Offset(s.HierarchyPositionOffset).Intersects(ray)) list.Add(s);
			}
			return list;
		}

		public List<Light> GetContributingLights(Vector3 pos) {
			var list = new List<Light>();
			for(int i = 0; i < lights.Count; i++) {
				if(lights[i].Contributes(pos)) list.Add(lights[i]);
			}
			return list;
		}

		public bool IsInWorldBounds(Vector3 pos) {
			return worldAABB.IsInside(pos);
		}

		public Shape[] GetAABBIntersectingShape(Vector3 pos) {
			return GetAABBIntersectingShapes(pos, shapes);
		}

		public Shape[] GetAABBIntersectingShapes(Vector3 pos, List<Shape> query) {
			List<Shape> list = new List<Shape>();
			if(sceneObjectsAABB.IsInside(pos)) {
				foreach(var s in query) {
					if(s.ExpandedAABB.IsInside(pos)) list.Add(s);
				}
			}
			return list.ToArray();
		}

		public Color SampleSkybox(Vector3 direction) {
			if(skyboxTexture != null) {
				if(skyboxIsSpherical) {
					var x = MathUtils.DirToEuler(direction).Y / 360f;
					var y = 0.5f + direction.Y / 2f;
					return skyboxTexture.Sample(x, y);
				} else {
					var euler = MathUtils.DirToEuler(direction);
					var x = euler.Y / 90f;
					x += 0.5f;
					var y = euler.X / 90f;
					y += 0.5f;
					return skyboxTexture.Sample(x, y);
				}
			} else {
				return skyboxGradient.Evaluate(direction.Y / 2f + 0.5f);
			}
		}
	}
}
