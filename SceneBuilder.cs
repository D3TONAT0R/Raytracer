using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Raytracer {
	public class SceneBuilder {

		public static Color[] randomColors = new Color[] {
			System.Drawing.Color.Red,
			System.Drawing.Color.DarkRed,
			System.Drawing.Color.IndianRed,
			System.Drawing.Color.Orange,
			System.Drawing.Color.DarkOrange,
			System.Drawing.Color.Yellow,
			System.Drawing.Color.LightGoldenrodYellow,
			System.Drawing.Color.GreenYellow,
			System.Drawing.Color.Green,
			System.Drawing.Color.ForestGreen,
			System.Drawing.Color.RosyBrown,
			System.Drawing.Color.SandyBrown,
			System.Drawing.Color.Cyan,
			System.Drawing.Color.DarkCyan,
			System.Drawing.Color.Blue,
			System.Drawing.Color.LightBlue,
			System.Drawing.Color.RoyalBlue,
			System.Drawing.Color.MediumPurple,
			System.Drawing.Color.Magenta,
			System.Drawing.Color.White,
			System.Drawing.Color.Gray,
			System.Drawing.Color.DarkGoldenrod,
			System.Drawing.Color.DarkGray
		};

		public static Scene Generate(int index) {
			if(index == 0) {
				return new SceneFileLoader().CreateFromFile(System.IO.File.ReadAllLines(System.IO.Path.Combine(RaytracedRenderer.rootPath, "scene_4.txt")), 0);
			} else if(index == 1) {
				return GenerateSphereWorld();
			} else if(index == 2) {
				return GenerateRandomWorld();
			} else if(index == 3) {
				var sn = new Scene(index);
				sn.AddObject(new Cuboid("floor", new Vector3(-10, -1, -10), new Vector3(20, 1, 20)));
				sn.AddObject(new Cuboid("cube", new Vector3(-2, 0, 4), Vector3.One));
				sn.AddObject(new Cuboid("cube", new Vector3(-1, 0, 2), Vector3.One));
				sn.AddObject(new Cuboid("cube", new Vector3(2, 0, 5), Vector3.One));
				sn.AddObject(new Cuboid("cube", new Vector3(1, 0, 6), Vector3.One));
				sn.AddObject(new Cuboid("cube", new Vector3(5, 0, 1), Vector3.One));
				sn.AddObject(new Cuboid("cube", new Vector3(-2, 0, -3), Vector3.One));
				sn.AddObject(new Cuboid("cube", new Vector3(2, 0, -1), Vector3.One));
				var sphere = new Sphere("sphere", Vector3.Zero, 0.25f) {
					material = new Material(Color.Red, 0, 0)
				};
				var group = new Group("group", Vector3.Zero, sphere, Light.CreatePointLight(new Vector3(0, 2.5f, 0), 15f, 1.5f, new Color(1, 0.6f, 0.2f)));
				sn.AddObject(group);
				sn.remoteControlledObject = group;
				return sn;
			} else if(index == 4) {
				var sn = new Scene(index);
				float fr = 0f;
				float fs = 0f;
				Material roadMaterial = Material.CreateTexturedMaterial("road", fr, fs, new TilingVector(0, 0, 0.2f, 0.2f, 90f));
				roadMaterial.mappingType = TextureMappingType.LocalYProj;
				Material sidewalkMaterial = Material.CreateTexturedMaterial("sidewalk", fr, fs, new TilingVector(0, 0, 0.2f, 0.2f, 0));
				sidewalkMaterial.mappingType = TextureMappingType.LocalYProj;
				Material building1Material = Material.CreateTexturedMaterial("building_1", 0, 0, TilingVector.FromSize(4));
				Material building2Material = Material.CreateTexturedMaterial("building_2", 0, 0, TilingVector.FromSize(4));
				Material checkerboardMaterial = Material.CreateCheckerMaterial(Color.White, Color.Gray, 0.5f, 0.9f, 4);
				Material roofMaterial = Material.CreateTexturedMaterial("roof", 0, 0, TilingVector.FromSize(2.5f));
				Material metal = new Material(Color.Gray, 0.5f, 0.7f);
				sn.AddObject(new Cuboid("road", new Vector3(0, -1, -10), new Vector3(5, 1, 30)) {
					material = roadMaterial
				});
				sn.AddObject(new Cuboid("floor_l", new Vector3(5, -1, -10), new Vector3(10, 1, 30)) {
					material = sidewalkMaterial
				});
				sn.AddObject(new Cuboid("floor_r", new Vector3(-10, -1, -10), new Vector3(10, 1, 30)) {
					material = sidewalkMaterial
				});
				sn.AddObjects(CreateHouse("house1", new Vector3(-5f, 0, 3), new Vector3(5, 4, 8), 2, Roof.RoofType.Hipped, building1Material, roofMaterial));
				sn.AddObjects(CreateHouse("house2", new Vector3(8, 0, -3), new Vector3(7, 6, 12), 1, Roof.RoofType.GableZ, building2Material, roofMaterial));
				sn.AddObject(Light.CreatePointLight(new Vector3(7.5f, 3, 5), 10, 1.5f, new Color(1, 0.6f, 0.2f)));
				var sphere = new Sphere("sphere", new Vector3(6.5f, 0.5f, 3.5f), 0.5f) {
					material = new Material(Color.Red, 0.6f, 0.8f)
				};
				sn.AddObject(sphere);
				var c1 = new Cuboid("base", new Vector3(-0.15f, 0, -0.15f), new Vector3(0.3f, 0.1f, 0.3f)) {
					material = metal
				};
				var c2 = new Cylinder("pole", Vector3.Zero, 0.075f, 2) {
					material = metal
				};
				var pos = Vector3.UnitY * 2.2f;
				var c3 = new Sphere("ball", pos, 0.25f);
				var cL = Light.CreatePointLight(pos + Vector3.UnitY, 15, 4, new Color(0.6f, 0.7f, 1f));
				cL.shadowStartOffset = 0.26f;
				sn.AddObject(new Group("group", new Vector3(0, 0, 0), c1, c2, c3, cL));
				sn.AddDefaultDirectionalLight();
				sn.fogDistance = 160f;

				//Animation
				var prop = new AnimatedProperty(RaytracedRenderer.instance.camera, "POSITION",
					new AnimatedProperty.Keyframe[] {
						new AnimatedProperty.Keyframe(0, new Vector3(-5f, 5f, -10f)),
						new AnimatedProperty.Keyframe(1, new Vector3(0f, 2f, -5f)),
						new AnimatedProperty.Keyframe(3, new Vector3(2f, 1f, 3))
					}
				);
				var prop2 = new AnimatedProperty(RaytracedRenderer.instance.camera, "ROTATION",
					new AnimatedProperty.Keyframe[] {
						new AnimatedProperty.Keyframe(0, new Vector3(25, 30, 30)),
						new AnimatedProperty.Keyframe(1, new Vector3(5, 0, 10)),
						new AnimatedProperty.Keyframe(3, new Vector3(-15, 45, 0))
					}
				);
				var animsphere = new AnimatedProperty(sphere, "POSITION",
					new AnimatedProperty.Keyframe[] {
						new AnimatedProperty.Keyframe(0.0f, new Vector3(6.5f, 0.5f, 3.5f)),
						new AnimatedProperty.Keyframe(0.5f, new Vector3(6.5f, 2.5f, 3.5f)),
						new AnimatedProperty.Keyframe(1.0f, new Vector3(6.5f, 0.5f, 3.5f)),
						new AnimatedProperty.Keyframe(1.5f, new Vector3(6.5f, 2.5f, 3.5f)),
						new AnimatedProperty.Keyframe(2.0f, new Vector3(6.5f, 0.5f, 3.5f)),
						new AnimatedProperty.Keyframe(2.5f, new Vector3(6.5f, 2.5f, 3.5f)),
						new AnimatedProperty.Keyframe(3.0f, new Vector3(6.5f, 0.5f, 3.5f)),
					}
				);
				var animsphere2 = new AnimatedProperty(sphere, "RADIUS",
					new AnimatedProperty.Keyframe[] {
						new AnimatedProperty.Keyframe(0.0f, 0.5f),
						new AnimatedProperty.Keyframe(1.5f, 1f),
						new AnimatedProperty.Keyframe(3f, 0.5f),
					}
				);

				Animator.animatedProperties.Add(prop);
				Animator.animatedProperties.Add(prop2);
				Animator.animatedProperties.Add(animsphere);
				Animator.animatedProperties.Add(animsphere2);
				return sn;
			} else {
				return null;
			}
		}

		static Scene GenerateRandomWorld() {
			var rand = new Random();
			var scene = new Scene(2);
			var materials = new Material[] {
				new Material(Color.White, 0.1f, 1) {
					mainTexture = Sampler2D.Create("texture_1.png")
				},
				new Material(Color.White, 0.3f, 1) {
					mainTexture = Sampler2D.Create("texture_2.png")
				},
				new Material(new Color(0.4f, 0.4f, 0.35f), 0.7f, 1f),
				new Material(new Color(0.2f, 0.6f, 1, 1), 0.3f, 1f)
			};
			for(int i = 0; i < 15; i++) {
				var color = randomColors[rand.Next(randomColors.Length)];
				if(rand.NextDouble() < 0.3f) {
					color.a = 0.25f + (float)rand.NextDouble() * 0.5f;
				}
				scene.AddObject(new Cuboid("cube " + i, new Vector3(rand.Next(-10, 10), rand.Next(-10, 10), rand.Next(-10, 10)), new Vector3(rand.Next(1, 4), rand.Next(1, 4), rand.Next(1, 4))) {
					material = materials[rand.Next(materials.Length)]
				});
			}
			return scene;
		}

		static Scene GenerateSphereWorld() {
			var scene = new Scene(1);
			scene.AddObject(new Sphere("sphere1", new Vector3(2, 2, 5), 2) {
				material = new Material(System.Drawing.Color.White, 0, 1) {
					mainTexture = Sampler2D.Create("texture_1.png")
				},
			});
			scene.AddObject(new Sphere("sphere2", new Vector3(1, 7, 6), 1) {
				material = new Material(System.Drawing.Color.LightBlue, 0.5f, 1)
			});
			scene.AddObject(new Sphere("sphere3", new Vector3(5, 2, 10), 3) {
				material = new Material(System.Drawing.Color.Yellow, 0.5f, 1) {
					mainTexture = Sampler2D.Create("texture_2.png")
				},
			});
			scene.AddObject(new Sphere("sphere4", new Vector3(-5, -2, 3), 4) {
				material = new Material(((Color)System.Drawing.Color.IndianRed).SetAlpha(0.5f), 0.5f, 1)
			});
			scene.AddObject(new Sphere("sphere5", new Vector3(1, -5, 2), 2) {
				material = new Material(System.Drawing.Color.Gray, 0.5f, 1)
			});
			scene.AddObject(new Sphere("sphere6", new Vector3(0, 0, 9), 1) {
				material = new Material(System.Drawing.Color.White, 0.5f, 1)
			});
			scene.AddObject(new Cuboid("cube1", new Vector3(3, 0, 3), new Vector3(2, 2, 2)) {
				material = new Material(System.Drawing.Color.OrangeRed, 0.35f, 1)
			});
			scene.AddObject(new Cuboid("cube2", new Vector3(0, -3, 0), new Vector3(5, 1, 5)) {
				material = new Material(System.Drawing.Color.White, 0.5f, 0.7f) {
					mainTexture = Sampler2D.Create("floortiles.jpg"),
					textureTiling = new TilingVector(0, 0, 0.5f, 0.5f)
				},
			});
			var wallMaterial = new Material(Color.White, 0, 0) {
				mainTexture = Sampler2D.Create("building_1.png"),
				textureTiling = new TilingVector(0, 0, 0.5f, 0.5f)
			};
			var roofMaterial = new Material(Color.White, 0, 0) {
				mainTexture = Sampler2D.Create("texture_1.png")
			};
			var house = CreateHouse("house1", new Vector3(-2, -2, 0), new Vector3(3, 3, 5), 2, Roof.RoofType.GableZ, wallMaterial, roofMaterial);
			var b1cube = house[0];
			var b2sphere = new Sphere("b2sphere", new Vector3(0.75f, -1.5f, 1.5f), 1) {
				material = new Material(System.Drawing.Color.White, 0, 1) {
					mainTexture = Sampler2D.Create("texture_2.png")
				}
			};
			var b3prism = new Prism("b3prism", new Vector3(0, -1.5f, 3.5f), new Vector3(1, 1, 1), false) {
				material = new Material(System.Drawing.Color.White, 0, 1) {
					mainTexture = Sampler2D.Create("texture_1.png")
				}
			};
			scene.AddObject(new BooleanSolid("cut_house", BooleanSolid.BooleanOperation.Difference, b1cube, b2sphere, b3prism));
			scene.AddObject(house[1]);
			var house2 = CreateHouse("tower", new Vector3(-0.5f, 1, 2), new Vector3(1.5f, 2, 1.5f), 2, Roof.RoofType.Pyramid, wallMaterial, roofMaterial);
			scene.AddObjects(house2);
			scene.AddObject(new Cuboid("house2", new Vector3(4, -2, 0), new Vector3(1, 3, 5)) {
				material = new Material(System.Drawing.Color.White, 0, 1) {
					mainTexture = Sampler2D.Create("building_2.png"),
					textureTiling = new TilingVector(0, 0, 0.5f, 0.5f)
				},
			});
			/*scene.AddShape(new Cuboid() {
				pos = new Vector3(2, -2, 2),
				size = new Vector3(1, 0.5f, 1),
				material = new Material(System.Drawing.Color.White, 0, 1) {
					mainTexture = Sampler2D.Create("texture_2.png")
				},
			});*/
			var redSphere = new Sphere("rsphere", new Vector3(2, -1.5f, 2), 0.5f) {
				material = new Material(new Color(1, 0.25f, 0, 0.3f), 0.75f, 1)
			};
			var blueSphere = new Sphere("bsphere", new Vector3(2, -1.5f, 1.5f), 0.25f) {
				material = new Material(new Color(0.25f, 0.25f, 1f), 0.75f, 1)
			};
			scene.AddObject(new BooleanSolid("cutsphere", BooleanSolid.BooleanOperation.Difference, redSphere, blueSphere));

			scene.AddObject(new Cylinder("cyl_y", new Vector3(5, 0, 1), 0.5f, 2f, Cylinder.CylinderAxis.Y) {
				material = new Material(Color.Green, 0.5f, 1)
			});
			scene.AddObject(new Cylinder("cyl_x", new Vector3(6.5f, 0, 0), 0.5f, 2f, Cylinder.CylinderAxis.X) {
				material = new Material(Color.Red, 0.5f, 1)
			});
			scene.AddObject(new Cylinder("cyl_z", new Vector3(3.5f, 1, -1), 0.5f, 2f, Cylinder.CylinderAxis.Z) {
				material = new Material(Color.Blue, 0.5f, 1)
			});

			scene.AddDefaultDirectionalLight();
			var pl = Light.CreatePointLight(new Vector3(1.8f, -1f, 4f), 8f, 1.5f, new Color(1f, 0.8f, 0.5f));
			scene.AddObject(pl);
			return scene;
		}

		public static SolidShape[] CreateHouse(string name, Vector3 pos, Vector3 size, float roofHeight, Roof.RoofType roofType, Material wallMaterial, Material roofMaterial) {
			var house = new Cuboid(name, pos, size) {
				material = wallMaterial
			};
			pos.Y += size.Y;
			size.Y = roofHeight;
			var roof = new Roof(name + "_roof", pos, size, roofType, roofMaterial) {
				material = wallMaterial
			};
			return new SolidShape[] { house, roof };
		}
	}
}
